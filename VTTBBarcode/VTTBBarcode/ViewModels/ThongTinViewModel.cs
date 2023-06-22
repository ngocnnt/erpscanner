using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using VTTBBarcode.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.ViewModels
{
    public class ThongTinViewModel : BaseViewModel
    {
        public Command ScanCommand { get; }
        //private DataHandler dataAccess;
        string _maCode;
        public string MaCode { get => _maCode; set => SetProperty(ref _maCode, value); }
        string _tThaiCTo;
        public string TThaiCTo { get => _tThaiCTo; set => SetProperty(ref _tThaiCTo, value); }
        string _tThaiKDinh;
        public string TThaiKDinh { get => _tThaiKDinh; set => SetProperty(ref _tThaiKDinh, value); }
        //string _ngayBDong;
        //public string NgayBDong { get => _ngayBDong; set => SetProperty(ref _ngayBDong, value); }
        //ObservableCollection<LichSuBD> _listLichSuBD;
        //public ObservableCollection<LichSuBD> ListLichSuBD
        //{
        //    get { return _listLichSuBD; }
        //    set
        //    {
        //        _listLichSuBD = value;
        //        OnPropertyChanged("ListLichSuBD");
        //    }
        //}
        ObservableCollection<BDongCTo> _lichSuBD;
        public ObservableCollection<BDongCTo> LichSuBD
        {
            get { return _lichSuBD; }
            set
            {
                _lichSuBD = value;
                OnPropertyChanged("LichSuBD");
            }
        }

        public ThongTinViewModel()
        {
            Title = "Kiểm tra thông tin công tơ";
            ScanCommand = new Command(OnScanCLicked);
            //this.dataAccess = new DataHandler();
        }
        private async void OnScanCLicked(object obj)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            ZXingScannerView zxing = new ZXingScannerView();
            ZXing.Result result = null;
            TimeSpan ts = new TimeSpan(0, 0, 0, 3, 0);
            Device.StartTimer(ts, () => {
                if (zxing.IsScanning)
                    zxing.AutoFocus();
                return true;
            });
            result = await scanner.Scan();
            if (result == null) return;  //user bấm back
            string type = result.BarcodeFormat.ToString();
            if (result != null)
            {
                DependencyService.Get<ISound>().playBeepSound();
                MaCode = result.Text;
                if (!CheckInternet())
                {
                    return;
                }
                try
                {
                    ShowLoading("Đang kiểm tra vui lòng đợi");
                    await Task.Delay(200);
                    string data = "";
                    if ((result.BarcodeFormat != ZXing.BarcodeFormat.CODE_39) && (result.BarcodeFormat != ZXing.BarcodeFormat.CODE_128))
                    {
                        DependencyService.Get<IToast>().Show(string.Format("Mã code không hợp lệ. Anh/ chị vui lòng kiểm tra lại!", Title));
                        return;
                    }
                    else data = result.Text;
                    if (data == "")
                    {
                        HideLoading();
                        return;
                    }
                    if (UserNameLogin == "ngocntt" && PassLogin == "1245678")
                    {
                        HideLoading();
                        TThaiCTo = "Chưa có lịch sử";
                        TThaiKDinh = "Chưa kiểm định";
                    }
                    else
                    {
                        //lấy trạng thái công tơ
                        var response = await client.GetAsync(Url + "CongTos/" + data);
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        ObservableCollection<CongTo> contents = JsonConvert.DeserializeObject<ObservableCollection<CongTo>>(responseContent);
                        //lấy lịch sử BĐ
                        response = await client.GetAsync(Url + "LichSuCToes/" + data);
                        responseContent = response.Content.ReadAsStringAsync().Result;
                        LichSuCTo contentsBD = JsonConvert.DeserializeObject<LichSuCTo>(responseContent);
                        HideLoading();
                        if ((contents == null) || (contents.Count != 1)) return;
                        TThaiCTo = contents[0].vttB_Status;
                        TThaiKDinh = (contents[0].checkedResult == true) ? "Kiểm định đạt" : (contents[0].checkedResult == false) ? "Kiểm định không đạt" : "Chưa kiểm định";
                        var kq = contentsBD.lichSu.OrderByDescending(a => a.ngaY_BDONG).ToList();
                        LichSuBD = new ObservableCollection<BDongCTo>(kq);
                    }
                }
                catch (Exception ex)
                {
                    HideLoading();
                }
                finally
                {
                }
            }
        }        
    }
}