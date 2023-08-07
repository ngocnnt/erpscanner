using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.SfDataGrid.XForms.Exporting;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using static VTTBBarcode.Models.clsVaribles;
using VTTBBarcode.Dialog;
using RestSharp;
using VTTBBarcode.Views;

namespace VTTBBarcode.ViewModels
{
    public class KiemKeViewModel : BaseViewModel
    {
        Page2 _myModalPage;
        public Command ScanCommand { get; }
        public Command SaveCommand { get; }
        public Command ExportCommand { get; }
        public Command DataCommand { get; }
        public SfDataGrid sfDataGrid { get; set; }
        //private DataHandler dataAccess;
        string _maCode;
        public string MaCode { get => _maCode; set => SetProperty(ref _maCode, value); }
        string _selectedVTTB;
        public string SelectedVTTB { get => _selectedVTTB; set => SetProperty(ref _selectedVTTB, value); }
        ObservableCollection<KiemKeTable> _listTemp;
        ObservableCollection<KiemKeTable> _kKTable;
        public ObservableCollection<KiemKeTable> KKTable
        {
            get { return _kKTable; }
            set
            {
                _kKTable = value;
                OnPropertyChanged("KKTable");
            }
        }

        ObservableCollection<DanhSachKho> _listKho;
        public ObservableCollection<DanhSachKho> ListKho
        {
            get { return _listKho; }
            set
            {
                _listKho = value;
                OnPropertyChanged("ListKho");
            }
        }
        public DanhSachKho _selectedKho;
        public DanhSachKho SelectedKho
        {
            get => _selectedKho;
            set => SetProperty(ref _selectedKho, value);
        }
        ObservableCollection<DanhSachKhoSub> _listKhoPhu;
        public ObservableCollection<DanhSachKhoSub> ListKhoPhu
        {
            get { return _listKhoPhu; }
            set
            {
                _listKhoPhu = value;
                OnPropertyChanged("ListKhoPhu");
            }
        }
        public DanhSachKhoSub _selectedKhoPhu;
        public DanhSachKhoSub SelectedKhoPhu
        {
            get => _selectedKhoPhu;
            set => SetProperty(ref _selectedKhoPhu, value);
        }

        int s = 0;
        public DanhSachOnHand listOnHandCheck;

        //ERP
        public SfDataGrid sfDataGridERP { get; set; }
        ObservableCollection<KiemKeTableExcel> _kKTableERP;
        public ObservableCollection<KiemKeTableExcel> KKTableERP
        {
            get { return _kKTableERP; }
            set
            {
                _kKTableERP = value;
                OnPropertyChanged("KKTableERP");
            }
        }

        public KiemKeViewModel()
        {
            Title = "Kiểm kê";
            SelectedVTTB = "3.60.05.130";
            ListKho = listKho;
            ScanCommand = new Command(OnScanCLicked);
            SaveCommand = new Command(OnSaveCLicked);
            ExportCommand = new Command(OnExportCLicked);
            DataCommand = new Command(OnDataCLicked);
        }
        private async void OnDataCLicked(object obj)
        {
            try
            {
                dataAccess.DeleteKiemKe(SelectedKho.MAKHO, SelectedKhoPhu.SECONDARY_INVENTORY_NAME);
                LoadData();
            }

            catch (Exception ex)
            {
            }
        }

        private async void OnScanCLicked(object obj)
        {
            if (SelectedKho == null || SelectedKhoPhu == null)
            {
                DependencyService.Get<IToast>().Show("Vui lòng chọn các thông tin kho trước khi thực hiện quét!");
                return;
            }
            if (Device.RuntimePlatform == Device.Android && Xamarin.Essentials.DeviceInfo.Version.Major < 8)
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                ZXingScannerView zxing = new ZXingScannerView();
                ZXing.Result result = null;
                TimeSpan ts = new TimeSpan(0, 0, 0, 3, 0);
                Device.StartTimer(ts, () =>
                {
                    if (zxing.IsScanning)
                        zxing.AutoFocus();
                    return true;
                });
                result = await scanner.Scan();
                if (result == null) return;
                string type = result.BarcodeFormat.ToString();
                if (result != null)
                {
                    DependencyService.Get<ISound>().playBeepSound();
                    ShowResult(result.Text, result.BarcodeFormat.ToString());
                }
            }
            else
            {
                App.Current.ModalPopping += HandleModalPopping;
                _myModalPage = new Page2();
                await App.Current.MainPage.Navigation.PushModalAsync(_myModalPage);
            }
        }
        private async void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == _myModalPage)
            {
                // now we can retrieve that phone number:
                var result = _myModalPage.data;
                var result1 = _myModalPage.format;
                _myModalPage = null;

                // remember to remove the event handler:
                App.Current.ModalPopping -= HandleModalPopping;
                if (result != null && result != "")
                {
                    DependencyService.Get<ISound>().playBeepSound();
                    ShowResult(result, result1);
                }
            }
        }

        private async void ShowResult(string result, string format)
        {
            MaCode = result;
            if (!CheckInternet())
            {
                return;
            }
            try
            {
                ShowLoading("Đang kiểm tra vui lòng đợi");
                await Task.Delay(200);
                string data = "";
                //format.ToUpper() == "QR_CODE" || format.ToUpper() == "QRCODE"
                //if (!(format.ToUpper() == "CODE_39" || format.ToUpper() == "CODE_128" || format.ToUpper() == "CODE39" || format.ToUpper() == "CODE128"))
                if (result.Contains("-") || result.Contains("_"))
                {
                    DependencyService.Get<IToast>().Show(string.Format("Mã code không hợp lệ. Anh/ chị vui lòng kiểm tra lại!", Title));
                    HideLoading();
                    return;
                }
                else data = result;
                if (data == "")
                {
                    HideLoading();
                    return;
                }
                //lấy trạng thái công tơ
                var response = await client.GetAsync(Url + "CongTos/" + data);
                var responseContent = response.Content.ReadAsStringAsync().Result;
                ObservableCollection<CongTo> contents = JsonConvert.DeserializeObject<ObservableCollection<CongTo>>(responseContent);
                HideLoading();

                //check với listOnhand
                bool ketqua = false;
                listOnHandCheck = listOnHand.Where(o => o.SERIAL_NUMBER == data).FirstOrDefault();
                if (contents[0].kho != SelectedKho.MAKHO || contents[0].kho_Phu != SelectedKhoPhu.SECONDARY_INVENTORY_NAME)
                {
                    DependencyService.Get<IToast>().Show(string.Format("VTTB {0} không nằm trong kho đang kiểm kê.", data));
                }
                else
                {
                    switch (listOnHandCheck.MACL)
                    {
                        case "000":
                            if (contents[0].checkedResult == true) ketqua = true;
                            break;
                        case "A70":
                            if (contents[0].checkedResult == true) ketqua = true;
                            break;
                        case "C70":
                            if (contents[0].checkedResult == null) ketqua = true;
                            break;
                        case "D50":
                            if (contents[0].checkedResult == false) ketqua = true;
                            break;
                        default:
                            break;
                    }
                    if (!ketqua)
                    {
                        DependencyService.Get<IToast>().Show(string.Format("VTTB {0}: kiểm định {1}.", data, (contents[0].checkedResult == true) ? "Đạt" : (contents[0].checkedResult == false) ? "Không đạt" : "Chưa kiểm định"));
                        //return;
                    }
                }
                bool itemExists = KKTable.Any(item =>
                {
                    return (item.MaCode == result);
                    //(item.MaKho == SelectedKho.MAKHO) &&
                    //(item.MaKhoPhu == SelectedKhoPhu.SECONDARY_INVENTORY_NAME);
                });
                if (!itemExists)
                {
                    s++;
                    KiemKeTable dt = new KiemKeTable();
                    dt.User = UserName;
                    dt.MaCode = result;
                    dt.vttB_Status = contents[0].vttB_Status;
                    dt.MaVTTB = (listOnHandCheck != null) ? listOnHandCheck.SEGMENT1 : contents[0].code_ERP;
                    dt.MaKho = contents[0].kho;
                    dt.MaKhoPhu = contents[0].kho_Phu;
                    dt.NgayKK = DateTime.Now;
                    dt.STT = s;
                    dt.CheckedEXDate = (contents[0].checkedEXDate != null) ? contents[0].checkedEXDate.Value.ToString("dd/MM/yyyy") : "";
                    dt.CheckedResult = (contents[0].checkedResult == true) ? "Đạt" : (contents[0].checkedResult == false) ? "Không đạt" : "Chưa kiểm định";
                    dt.DVT = (listOnHandCheck != null) ? listOnHandCheck.DVT : "Cái";
                    dt.SL = (listOnHandCheck != null) ? listOnHandCheck.ON_HAND : "1";
                    dt.KetQua = ketqua ? "Đã kiểm kê khớp" : "Đã kiểm kê không khớp";
                    KKTable.Add(dt);
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

        private void LoadData()
        {
            _listTemp = new ObservableCollection<KiemKeTable>(dataAccess.LoadRecordKiemKe(SelectedKho.MAKHO, SelectedKhoPhu.SECONDARY_INVENTORY_NAME));
            foreach (KiemKeTable t in _listTemp)
            {
                s++;
                t.STT = s;
            }
            KKTable = _listTemp;
        }

        private void OnSaveCLicked(object obj)
        {
            if ((KKTable == null) || (KKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách kiểm kê để lưu. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                try
                {
                    foreach (KiemKeTable ma in KKTable)
                    {
                        if (ma != null)
                        {
                            dataAccess.SaveRecordKiemKe(ma);
                        }
                    }
                    DependencyService.Get<IToast>().Show("Lưu dữ liệu thành công!");
                    //LoadData();
                }
                catch (Exception ex)
                {
                    HideLoading();
                }
            }
        }

        private async void OnExportCLicked(object obj)
        {
            if ((KKTable == null) || (KKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để xuất excel. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                try
                {
                    //tạo table erp và cmis
                    ShowLoading("Vui lòng đợi");
                    await Task.Delay(200);
                    exportERP();
                    HideLoading();
                    DependencyService.Get<IToast>().Show("Xuất excel thành công!");
                }
                catch (Exception ex)
                {
                    HideLoading();
                }
                //StoragePermission.Check();
                //DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
                //var excelEngine = excelExport.ExportToExcel(this.sfDataGrid);
                //var workbook = excelEngine.Excel.Workbooks[0];
                //MemoryStream stream = new MemoryStream();
                //workbook.SaveAs(stream);
                //workbook.Close();
                //excelEngine.Dispose();
                //await DependencyService.Get<ISave>().SaveAndView("ChuyenKho_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", "application/msexcel", stream, "");
                //DependencyService.Get<IToast>().Show("Xuất excel thành công!");
            }
        }

        ObservableCollection<ObservableCollection<ChuyenKhoTable>> Split(ObservableCollection<ChuyenKhoTable> collection, int splitBy = 10)
        {
            var result = collection
                       .Select((x, i) => new { index = i, item = x })
                       .GroupBy(x => x.index / splitBy, x => x.item)
                       .Select(g => new ObservableCollection<ChuyenKhoTable>(g));
            return new ObservableCollection<ObservableCollection<ChuyenKhoTable>>(result);
        }

        public async void loadKhoPhu()
        {
            var request = new RestRequest(UrlHD + "get_Org_Suborg", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("org_id", userInfo.ORG_ID);
            request.AddParameter("Password", PassListKho);
            RestResponse response = await clientS.ExecutePostAsync(request);
            var output = response.Content.Split('>', '<');
            ObservableCollection<DanhSachKhoSub> temp = JsonConvert.DeserializeObject<ObservableCollection<DanhSachKhoSub>>(output[4]);
            ListKhoPhu = new ObservableCollection<DanhSachKhoSub>(temp.Where(o => o.ORGANIZATION_CODE == SelectedKho.MAKHO));
            if (ListKhoPhu.Count == 1)
                SelectedKhoPhu = ListKhoPhu[0];   
        }

        public async void loadDSOnHand()
        {
            LoadData();
            var request = new RestRequest(UrlHD + "get_OnHand_Serial_by_Makho", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("makho", SelectedKho.MAKHO);
            request.AddParameter("Password", PassListKho);
            RestResponse response = await clientS.ExecutePostAsync(request);
            var output = response.Content.Split('>', '<');
            listOnHand = JsonConvert.DeserializeObject<ObservableCollection<DanhSachOnHand>>(output[4]);            
        }

        public async void exportERP()
        {
            try
            {
                KKTableERP = new ObservableCollection<KiemKeTableExcel>();
                foreach (KiemKeTable ct in KKTable)
                {
                    KiemKeTableExcel ds = new KiemKeTableExcel();
                    ds.Column1 = ct.STT.ToString();
                    ds.Column2 = ct.NgayKK.ToString();
                    ds.Column3 = ct.MaKho;
                    ds.Column4 = ct.MaKhoPhu;
                    ds.Column5 = ct.MaVTTB;
                    ds.Column6 = ct.DVT;
                    ds.Column7 = ct.MaCode;
                    ds.Column8 = ct.SL;
                    ds.Column9 = ct.KetQua;
                    ds.Column10 = ct.CheckedResult;
                    ds.Column11 = ct.CheckedEXDate;
                    ds.Column12 = ct.vttB_Status;
                    KKTableERP.Add(ds);
                }
                //lấy list chưa kiểm kê
                var result = listOnHand.Where(p => KKTable.All(p2 => p2.MaCode != p.SERIAL_NUMBER));
                int ss = KKTableERP.Count;
                foreach (DanhSachOnHand ct in result)
                {
                    ss++;
                    KiemKeTableExcel ds = new KiemKeTableExcel();
                    ds.Column1 = ss.ToString();
                    ds.Column2 = "";
                    ds.Column3 = ct.ORGANIZATION_CODE;
                    ds.Column4 = ct.SUBINVENTORY_CODE;
                    ds.Column5 = ct.SEGMENT1;
                    ds.Column6 = ct.DVT;
                    ds.Column7 = ct.SERIAL_NUMBER;
                    ds.Column8 = ct.ON_HAND;
                    ds.Column9 = "Chưa kiểm kê";
                    ds.Column10 = "";
                    ds.Column11 = "";
                    ds.Column12 = "";
                    KKTableERP.Add(ds);
                }

                //xuất excel
                StoragePermission.Check();
                DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
                var excelEngine = excelExport.ExportToExcel(this.sfDataGridERP);
                var workbook = excelEngine.Excel.Workbooks[0];
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                workbook.Close();
                excelEngine.Dispose();
                string filename = UserName + "-" + KKTable[0].MaKho + "-" + KKTable[0].MaKhoPhu + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                await DependencyService.Get<ISave>().SaveAndView(filename, "application/msexcel", stream, "KiemKe");
            }
            catch (Exception ex)
            {
                HideLoading();
            }
        }
    }
}