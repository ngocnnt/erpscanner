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
using ZXing.Common;
using Plugin.Screenshot;

namespace VTTBBarcode.ViewModels
{
    public class XuatKhoViewModel : BaseViewModel
    {
        public Command ScanCommand { get; }
        public Command SaveCommand { get; }
        public Command DataCommand { get; }
        public Command AcceptCommand { get; }
        public Command DeclineCommand { get; }
        public Command ExportCommand { get; }
        public SfDataGrid sfDataGrid { get; set; }
        //private DataHandler dataAccess;
        string _maCode;
        public string MaCode { get => _maCode; set => SetProperty(ref _maCode, value); }
        string _maVTTB;
        public string MaVTTB { get => _maVTTB; set => SetProperty(ref _maVTTB, value); }
        ObservableCollection<XuatKhoTable> _listTemp;
        ObservableCollection<XuatKhoTable> _xKTable;
        public ObservableCollection<XuatKhoTable> XKTable
        {
            get { return _xKTable; }
            set
            {
                _xKTable = value;
                OnPropertyChanged("XKTable");
            }
        }

        //ObservableCollection<DanhSachKho> _listKHXuat;
        //public ObservableCollection<DanhSachKho> ListKHXuat
        //{
        //    get { return _listKHXuat; }
        //    set
        //    {
        //        _listKHXuat = value;
        //        OnPropertyChanged("ListKHXuat");
        //    }
        //}
        //public DanhSachKho _selectedKH;
        //public DanhSachKho SelectedKH
        //{
        //    get => _selectedKH;
        //    set => SetProperty(ref _selectedKH, value);
        //}

        string _selectedKH;
        public string SelectedKH { get => _selectedKH; set => SetProperty(ref _selectedKH, value); }

        string _khoTon;
        public string KhoTon { get => _khoTon; set => SetProperty(ref _khoTon, value); }
        string _selectedHTXK;
        public string SelectedHTXK { get => _selectedHTXK; set => SetProperty(ref _selectedHTXK, value); }

        ObservableCollection<CongTo> _dSThung;
        public ObservableCollection<CongTo> DSThung
        {
            get { return _dSThung; }
            set
            {
                _dSThung = value;
                OnPropertyChanged("DSThung");
            }
        }
        bool _openPopupThung;
        public bool IsOpenPopupThung { get => _openPopupThung; set => SetProperty(ref _openPopupThung, value); }
        //bool _isAcceptSend;
        ////public bool IsAcceptSend { get => _isAcceptSend; set => SetProperty(ref _isAcceptSend, value); }
        //public bool IsAcceptSend
        //{
        //    get
        //    {
        //        return _isAcceptSend;
        //    }
        //    set
        //    {
        //        _isAcceptSend = value;
        //        OnPropertyChanged();
        //    }
        //}

        bool _chophepTH = true;
        int s = 0;
        int idFile = -1;
        string dsSerial = "";
        string fileNameex = "";

        //ERP
        public SfDataGrid sfDataGridERP { get; set; }
        ObservableCollection<NhapKhoTableERP> _xKTableERP;
        public ObservableCollection<NhapKhoTableERP> XKTableERP
        {
            get { return _xKTableERP; }
            set
            {
                _xKTableERP = value;
                OnPropertyChanged("XKTableERP");
            }
        }

        //show QRcode
        bool _isOpenPopupQRCode;
        public bool IsOpenPopupQRCode { get => _isOpenPopupQRCode; set => SetProperty(ref _isOpenPopupQRCode, value); }
        string _imgUrl;
        public string ImgUrl { get => _imgUrl; set => SetProperty(ref _imgUrl, value); }
        public ZXingBarcodeImageView QRCodeGen { get; set; }
        public EncodingOptions BarcodeOptions => new EncodingOptions() { Height = 500, Width = 500, PureBarcode = true };

        public Command LichSuCommand { get; }

        public XuatKhoViewModel()
        {
            Title = "Xuất kho";
            IsOpenPopupThung = false;
            //IsAcceptSend = false;
            ScanCommand = new Command(OnScanCLicked);
            SaveCommand = new Command(OnSaveCLicked);
            ExportCommand = new Command(OnExportCLicked);
            DataCommand = new Command(OnDataCLicked, CanExecuteDataCommand);
            AcceptCommand = new Command(OnAcceptCLicked);
            DeclineCommand = new Command(OnDeclineCLicked);
            //this.dataAccess = new DataHandler();
            IsOpenPopupQRCode = false;
            LichSuCommand = new Command(OnLichSuCLicked);

            LoadData();
            //ListKhoChuyen = listKho;
        }
        bool CanExecuteDataCommand(object arg)
        {
            if ((XKTable == null) || (XKTable.Count == 0))
                return false;
            else return true;
        }

        private async void OnScanCLicked(object obj)
        {
            if (XKTable.Count >= 20)
            {
                DependencyService.Get<IToast>().Show("Lô thực hiện không được nhiều hơn 20 thiết bị!");
                return;
            }
            if (SelectedKH == null || SelectedHTXK == null)
            {
                DependencyService.Get<IToast>().Show("Vui lòng chọn các thông tin xuất kho trước khi thực hiện quét!");
                return;
            }
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            ZXingScannerView zxing = new ZXingScannerView();
            ZXing.Result result = null;
            IsOpenPopupThung = false;
            _chophepTH = true;
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
                    if (result.BarcodeFormat.ToString() == "QR_CODE")
                    {
                        //lấy ds cto từ thùng QRcode
                        if (result.Text.Contains("-"))
                        {
                            var dataT = new
                            {
                                cloai = result.Text,
                            };
                            var httpContentT = new StringContent(JsonConvert.SerializeObject(dataT), Encoding.UTF8, "application/json");
                            var responseT = await client.PostAsync(UrlThung + "barcode_check_sothung", httpContentT);
                            var responseContentT = responseT.Content.ReadAsStringAsync().Result;
                            data = responseContentT;
                            data = data.Replace("\"", "");
                        }
                        else
                        {
                            data = result.Text.Replace("_", ",");
                        }
                    }
                    else data = result.Text;
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
                    if (result.BarcodeFormat.ToString() == "QR_CODE")
                    {
                        DSThung = contents;
                        foreach (CongTo ct in DSThung)
                        {
                            if (ct.vttB_Status != TThai02)
                            {
                                DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial ở trạng thái không cho phép thực hiện '{0}'. Anh/ chị vui lòng kiểm tra lại!", Title));
                                _chophepTH = false;
                                return;
                            }
                        }
                        IsOpenPopupThung = true;
                    }
                    else
                    {
                        MaVTTB = "3.60.05.130.VIE.SE." +
                        ((SelectedHTXK == "Xuất phục vụ lắp mới" || SelectedHTXK == "Xuất phục vụ thay định kỳ") ? "000" : ((contents[0].checkedResult == true) ? "A70" : ((contents[0].checkedResult == false) ? "D50" : "C70")));
                        KhoTon = contents[0].ten_Kho;
                        if (contents[0].vttB_Status != TThai02)
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trạng thái serial {0}: {1}, không cho phép thực hiện '{2}'. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, Title));
                            return;
                        }
                        bool itemExists = XKTable.Any(item =>
                        {
                            return (item.MaCode == result.Text) &&
                                   (item.User == UserName);
                        });
                        if (!itemExists)
                        {
                            s++;
                            XuatKhoTable dt = new XuatKhoTable();
                            dt.User = UserName;
                            dt.MaCode = result.Text;
                            dt.vttB_Status = contents[0].vttB_Status;
                            dt.MaVTTB = MaVTTB;
                            dt.HinhThucXK = SelectedHTXK;
                            dt.KhoTon = contents[0].kho;
                            dt.KhoTonPhu = contents[0].kho_Phu;
                            dt.DoiTac = SelectedKH;
                            dt.NgayXL = DateTime.Now;
                            dt.STT = s;
                            XKTable.Add(dt);
                            DataCommand?.ChangeCanExecute();
                        }
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

        private async void OnAcceptCLicked(object obj)
        {
            MaVTTB = "3.60.05.130.VIE.SE." +
                        ((SelectedHTXK == "Xuất phục vụ lắp mới" || SelectedHTXK == "Xuất phục vụ thay định kỳ") ? "000" : ((DSThung[0].checkedResult == true) ? "A70" : ((DSThung[0].checkedResult == false) ? "D50" : "C70")));
            KhoTon = DSThung[0].ten_Kho;
            if (_chophepTH)
            {
                foreach (CongTo ct in DSThung)
                {                    
                    bool itemExists = XKTable.Any(item =>
                    {
                        return (item.MaCode == ct.serialNum) &&
                               (item.User == UserName);
                    });
                    if (!itemExists)
                    {
                        s++;
                        XuatKhoTable ds = new XuatKhoTable();
                        ds.User = UserName;
                        ds.MaCode = ct.serialNum;
                        ds.vttB_Status = ct.vttB_Status;
                        ds.MaVTTB = MaVTTB;
                        ds.HinhThucXK = SelectedHTXK;
                        ds.KhoTon = ct.kho;
                        ds.KhoTonPhu = ct.kho_Phu;
                        ds.DoiTac = SelectedKH;
                        ds.NgayXL = DateTime.Now;
                        ds.STT = s;
                        XKTable.Add(ds);
                        DataCommand?.ChangeCanExecute();
                    }
                }
            }
            IsOpenPopupThung = false;
        }
        private async void OnDeclineCLicked(object obj)
        {
            IsOpenPopupThung = false;
        }

        private void LoadData()
        {
            s = 0;
            _listTemp = new ObservableCollection<XuatKhoTable>(dataAccess.LoadRecordXuatKho(UserName, 0));
            foreach (XuatKhoTable t in _listTemp)
            {
                s++;
                t.STT = s;
            }
            XKTable = _listTemp;
            DataCommand?.ChangeCanExecute();
        }

        private void OnSaveCLicked(object obj)
        {
            if ((XKTable == null) || (XKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để lưu. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                foreach (XuatKhoTable ma in XKTable)
                {
                    if (ma != null)
                    {
                        dataAccess.SaveRecordXuatKho(ma);
                    }
                }
                DependencyService.Get<IToast>().Show("Lưu dữ liệu thành công!");
                //LoadData();
            }
        }

        private async void OnDataCLicked(object obj)
        {
            try
            {
                dataAccess.DeleteXuatKho(UserName);
                LoadData();
            }

            catch (Exception ex)
            {
                HideLoading();
            }
        }
        private async void OnExportCLicked(object obj)
        {
            if ((XKTable == null) || (XKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để xuất excel. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                try
                {
                    //lấy thông tin
                    var ok = await new MessageXacThuc("XK", XKTable[0].KhoTon).Show();
                    if (ok == DialogReturn.OK)
                    {
                        //Preferences.Set(Config.AprroveFinger, _toggledVanTay);
                    }
                    //tạo table erp và cmis
                    ShowLoading("Vui lòng đợi");
                    await Task.Delay(200);
                    exportERP();
                    HideLoading();
                    DependencyService.Get<IToast>().Show("Xuất excel thành công!");
                    //show mã QRCode lên mh
                    ImgUrl = dsSerial;
                    IsOpenPopupQRCode = true;
                    LoadData();
                    //lưu mã qrcode
                    var stream = new MemoryStream(await CrossScreenshot.Current.CaptureAsync());
                    await DependencyService.Get<ISave>().SaveAndView(fileNameex, "application/jpeg", stream, "QRCODE");
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
                //await DependencyService.Get<ISave>().SaveAndView("XuatKho_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", "application/msexcel", stream, "");
                //DependencyService.Get<IToast>().Show("Xuất excel thành công!");
            }
        }

        ObservableCollection<ObservableCollection<XuatKhoTable>> Split(ObservableCollection<XuatKhoTable> collection, int splitBy = 10)
        {
            var result = collection
                       .Select((x, i) => new { index = i, item = x })
                       .GroupBy(x => x.index / splitBy, x => x.item)
                       .Select(g => new ObservableCollection<XuatKhoTable>(g));
            return new ObservableCollection<ObservableCollection<XuatKhoTable>>(result);
        }
        private string getCodeBDong(string bd)
        {       
            switch (bd)
            {
                case "Xuất phục vụ lắp mới":
                    return "421";
                case "Xuất phục vụ thay định kỳ":
                    return "423";
                case "Xuất phục vụ XLSC":
                    return "422";
                case "Xuất gia công, sửa chữa, thí nghiệm":
                    return "371";
                case "Xuất thanh lý, nhượng bán":
                    return "36";
                case "Xuất đi bảo hành":
                    return "372";
                default:
                    return "";
            }
        }

        public async void exportERP()
        {
            ObservableCollection<ObservableCollection<XuatKhoTable>> groupSplit = new ObservableCollection<ObservableCollection<XuatKhoTable>>();
            groupSplit = Split(XKTable);
            for (int j = 0; j < groupSplit.Count; j++)
            {
                XKTableERP = new ObservableCollection<NhapKhoTableERP>();
                int index = 0;
                if (groupSplit[j].Count == 1)
                {
                    NhapKhoTableERP ds = new NhapKhoTableERP();
                    ds.Column1 = @"\{ENTER}";
                    ds.Column2 = @"*SL0,5";
                    ds.Column3 = @"\+{TAB}";
                    ds.Column4 = "%" + groupSplit[j][0].KhoTon + "%";
                    ds.Column5 = @"\{ENTER}";
                    ds.Column6 = @"\{DOWN}";
                    ds.Column7 = @"\{ENTER}";
                    ds.Column8 = "*SL0,5";
                    ds.Column9 = DateTime.Now.ToString();
                    ds.Column10 = @"\{TAB}";
                    ds.Column11 = "Account alias issue";
                    ds.Column12 = @"\{TAB}";
                    ds.Column13 = getReasonCodeD(getCodeBDong(groupSplit[j][0].HinhThucXK));
                    ds.Column14 = @"\{TAB}";
                    ds.Column15 = getReasonCodeD(getCodeBDong(groupSplit[j][0].HinhThucXK));
                    ds.Column16 = @"\{TAB}";
                    ds.Column17 = @"\^L";
                    ds.Column18 = @"\%O";
                    ds.Column19 = @"\{TAB}";
                    ds.Column20 = @"\^L";
                    ds.Column21 = @"\%O";
                    ds.Column22 = @"\{TAB}";
                    ds.Column23 = InfoPopup3;
                    ds.Column24 = @"\{TAB 4}";
                    ds.Column25 = "Issue Transaction Informations";
                    ds.Column26 = @"\{TAB}";
                    ds.Column27 = InfoPopup1;
                    ds.Column28 = @"\{TAB}";
                    if (groupSplit[j][0].HinhThucXK == "Xuất phục vụ lắp mới" || groupSplit[j][0].HinhThucXK == "Xuất phục vụ thay định kỳ")
                    {
                        ds.Column29 = InfoPopup2;
                        ds.Column30 = @"\{TAB 6}";
                        ds.Column31 = InfoPopup7;
                        ds.Column32 = @"\{TAB}";
                        ds.Column33 = InfoPopup8;
                    }
                    else
                    {
                        ds.Column29 = "";
                        ds.Column30 = "";
                        ds.Column31 = InfoPopup7;
                        ds.Column32 = @"\{TAB 6}";
                        ds.Column33 = InfoPopup6;
                    }
                    ds.Column34 = @"\{TAB 2}";
                    ds.Column35 = InfoPopup5;
                    ds.Column36 = @"\%O";
                    ds.Column37 = @"\%R";
                    ds.Column38 = "*SL0,5";
                    ds.Column39 = groupSplit[j][0].MaVTTB;
                    ds.Column40 = @"\{TAB}";
                    ds.Column41 = groupSplit[j][0].KhoTonPhu;
                    ds.Column42 = @"\{TAB 3}";
                    ds.Column43 = XKTable.Count.ToString();
                    ds.Column44 = @"\{TAB}";
                    ds.Column45 = @"\%R";
                    ds.Column46 = "*SL0,5";
                    ds.Column47 = @"\{TAB}";
                    ds.Column48 = @"\%O";
                    ds.Column49 = @"\%I";
                    ds.Column50 = groupSplit[j][0].MaCode;
                    ds.Column51 = @"\{TAB}";
                    ds.Column52 = "";
                    ds.Column53 = "";
                    ds.Column54 = @"\%D";
                    ds.Column55 = "*SL0,5";
                    ds.Column56 = @"\^S";
                    ds.Column57 = @"\%O";
                    ds.Column58 = @"\^{F4}";
                    ds.Column59 = @"\{UP}";
                    ds.Column60 = "*SL3";
                    XKTableERP.Add(ds);
                }
                else foreach (XuatKhoTable ct in groupSplit[j])
                {
                    if (index == 0)
                    {
                        NhapKhoTableERP ds = new NhapKhoTableERP();
                        ds.Column1 = @"\{ENTER}";
                        ds.Column2 = @"*SL0,5";
                        ds.Column3 = @"\+{TAB}";
                        ds.Column4 = "%" + ct.KhoTon + "%";
                        ds.Column5 = @"\{ENTER}";
                        ds.Column6 = @"\{DOWN}";
                        ds.Column7 = @"\{ENTER}";
                        ds.Column8 = "*SL0,5";
                        ds.Column9 = DateTime.Now.ToString();
                        ds.Column10 = @"\{TAB}";
                        ds.Column11 = "Account alias issue";
                        ds.Column12 = @"\{TAB}";
                        ds.Column13 = getReasonCodeD(getCodeBDong(ct.HinhThucXK));
                        ds.Column14 = @"\{TAB}";
                        ds.Column15 = getReasonCodeD(getCodeBDong(ct.HinhThucXK));
                        ds.Column16 = @"\{TAB}";
                        ds.Column17 = @"\^L";
                        ds.Column18 = @"\%O";
                        ds.Column19 = @"\{TAB}";
                        ds.Column20 = @"\^L";
                        ds.Column21 = @"\%O";
                        ds.Column22 = @"\{TAB}";
                        ds.Column23 = InfoPopup3;
                        ds.Column24 = @"\{TAB 4}";
                        ds.Column25 = "Issue Transaction Informations";
                        ds.Column26 = @"\{TAB}";
                        ds.Column27 = InfoPopup1;
                        ds.Column28 = @"\{TAB}";
                        if (ct.HinhThucXK == "Xuất phục vụ lắp mới" || ct.HinhThucXK == "Xuất phục vụ thay định kỳ")
                        {
                            ds.Column29 = InfoPopup2;
                            ds.Column30 = @"\{TAB 6}";
                            ds.Column31 = InfoPopup7;
                            ds.Column32 = @"\{TAB}";
                            ds.Column33 = InfoPopup8;
                        }
                        else
                        {
                            ds.Column29 = "";
                            ds.Column30 = "";
                            ds.Column31 = InfoPopup7;
                            ds.Column32 = @"\{TAB 6}";
                            ds.Column33 = InfoPopup6;
                        }
                        ds.Column34 = @"\{TAB 2}";
                        ds.Column35 = InfoPopup5;
                        ds.Column36 = @"\%O";
                        ds.Column37 = @"\%R";
                        ds.Column38 = "*SL0,5";
                        ds.Column39 = ct.MaVTTB;
                        ds.Column40 = @"\{TAB}";
                        ds.Column41 = ct.KhoTonPhu; 
                        ds.Column42 = @"\{TAB 3}";
                        ds.Column43 = XKTable.Count.ToString();
                        ds.Column44 = @"\{TAB}";
                        ds.Column45 = @"\%R";
                        ds.Column46 = "*SL0,5";
                        ds.Column47 = @"\{TAB}";
                        ds.Column48 = @"\%O";
                        ds.Column49 = @"\%I";
                        ds.Column50 = ct.MaCode;
                        ds.Column51 = "";
                        ds.Column52 = "";
                        ds.Column53 = "";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = @"\{DOWN}";
                        ds.Column57 = "";
                        ds.Column58 = "";
                        ds.Column59 = "";
                        ds.Column60 = "";
                        XKTableERP.Add(ds);
                    }
                    else if (index == groupSplit[j].Count - 1)
                    {
                        NhapKhoTableERP ds = new NhapKhoTableERP();
                        ds.Column1 = "";
                        ds.Column2 = "";
                        ds.Column3 = "";
                        ds.Column4 = "";
                        ds.Column5 = "";
                        ds.Column6 = "";
                        ds.Column7 = "";
                        ds.Column8 = "";
                        ds.Column9 = "";
                        ds.Column10 = "";
                        ds.Column11 = "";
                        ds.Column12 = "";
                        ds.Column13 = "";
                        ds.Column14 = "";
                        ds.Column15 = "";
                        ds.Column16 = "";
                        ds.Column17 = "";
                        ds.Column18 = "";
                        ds.Column19 = "";
                        ds.Column20 = "";
                        ds.Column21 = "";
                        ds.Column22 = "";
                        ds.Column23 = "";
                        ds.Column24 = "";
                        ds.Column25 = "";
                        ds.Column26 = "";
                        ds.Column27 = "";
                        ds.Column28 = "";
                        ds.Column29 = "";
                        ds.Column30 = "";
                        ds.Column31 = "";
                        ds.Column32 = "";
                        ds.Column33 = "";
                        ds.Column34 = "";
                        ds.Column35 = "";
                        ds.Column36 = "";
                        ds.Column37 = "";
                        ds.Column38 = "";
                        ds.Column39 = "";
                        ds.Column40 = "";
                        ds.Column41 = "";
                        ds.Column42 = "";
                        ds.Column43 = "";
                        ds.Column44 = "";
                        ds.Column45 = "";
                        ds.Column46 = "";
                        ds.Column47 = "";
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = ct.MaCode;
                        ds.Column51 = @"\{TAB}";
                        ds.Column52 = "";
                        ds.Column53 = "";
                        ds.Column54 = @"\%D";
                        ds.Column55 = "*SL0,5";
                        ds.Column56 = @"\^S";
                        ds.Column57 = @"\%O";
                        ds.Column58 = @"\^{F4}";
                        ds.Column59 = @"\{UP}";
                        ds.Column60 = "*SL3";
                        XKTableERP.Add(ds);
                    }
                    else
                    {
                        NhapKhoTableERP ds = new NhapKhoTableERP();
                        ds.Column1 = "";
                        ds.Column2 = "";
                        ds.Column3 = "";
                        ds.Column4 = "";
                        ds.Column5 = "";
                        ds.Column6 = "";
                        ds.Column7 = "";
                        ds.Column8 = "";
                        ds.Column9 = "";
                        ds.Column10 = "";
                        ds.Column11 = "";
                        ds.Column12 = "";
                        ds.Column13 = "";
                        ds.Column14 = "";
                        ds.Column15 = "";
                        ds.Column16 = "";
                        ds.Column17 = "";
                        ds.Column18 = "";
                        ds.Column19 = "";
                        ds.Column20 = "";
                        ds.Column21 = "";
                        ds.Column22 = "";
                        ds.Column23 = "";
                        ds.Column24 = "";
                        ds.Column25 = "";
                        ds.Column26 = "";
                        ds.Column27 = "";
                        ds.Column28 = "";
                        ds.Column29 = "";
                        ds.Column30 = "";
                        ds.Column31 = "";
                        ds.Column32 = "";
                        ds.Column33 = "";
                        ds.Column34 = "";
                        ds.Column35 = "";
                        ds.Column36 = "";
                        ds.Column37 = "";
                        ds.Column38 = "";
                        ds.Column39 = "";
                        ds.Column40 = "";
                        ds.Column41 = "";
                        ds.Column42 = "";
                        ds.Column43 = "";
                        ds.Column44 = "";
                        ds.Column45 = "";
                        ds.Column46 = "";
                        ds.Column47 = "";
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = ct.MaCode;
                        ds.Column51 = "";
                        ds.Column52 = "";
                        ds.Column53 = "";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = @"\{DOWN}";
                        ds.Column57 = "";
                        ds.Column58 = "";
                        ds.Column59 = "";
                        ds.Column60 = "";
                        XKTableERP.Add(ds);
                    }
                    index++;
                }
                //xuất excel
                StoragePermission.Check();
                DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
                DataGridExcelExportingOption exportOption = new DataGridExcelExportingOption();
                exportOption.ExportHeader = false;
                var excelEngine = excelExport.ExportToExcel(this.sfDataGridERP, exportOption);
                var workbook = excelEngine.Excel.Workbooks[0];
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                workbook.Close();
                excelEngine.Dispose();
                string filename = UserName + gettenfile(groupSplit[j][0].KhoTon, getCodeBDong(groupSplit[j][0].HinhThucXK), "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-ERP-" + (j + 1).ToString("00") + ".xlsx";
                await DependencyService.Get<ISave>().SaveAndView(filename, "application/msexcel", stream, "ERP");

                byte[] bytes = stream.ToArray();
                var client1 = new RestClient();
                var request = new RestRequest(UrlHD + "UploadFile_dataload_ftp", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("File_name", filename);
                for (int n = 0; n < bytes.Length; n++)
                {
                    request.AddParameter("f", bytes[n].ToString());
                }
                request.AddParameter("password", PassSendFile);
                RestResponse response = client1.Execute(request);
                var output = response.Content.Split('>', '<');

                //insert vào bảng FileDaGoi
                if (j == 0)
                {
                    foreach (XuatKhoTable ct in XKTable)
                    {
                        dsSerial += ct.MaCode + "_";
                    }
                    dsSerial = dsSerial.Substring(0, dsSerial.Length - 1);
                    FileDaGoiTable ma = new FileDaGoiTable();
                    ma.ChucNang = 4;
                    ma.MaQRCode = dsSerial;
                    ma.TenFile = filename.Substring(0, filename.IndexOf("-ERP-", StringComparison.Ordinal));
                    ma.User = UserName;
                    ma.NgayXL = DateTime.Now;
                    idFile = dataAccess.SaveRecordFile(ma);
                    foreach (XuatKhoTable ct in XKTable)
                    {
                        ct.IDFile = idFile;
                        dataAccess.SaveRecordXuatKho(ct);
                    }
                    fileNameex = ma.TenFile + ".jpg";
                }
            }
        }

        private void OnLichSuCLicked(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //Shell.Current.Navigation.PushModalAsync(new LichSuPage(4));
                Shell.Current.Navigation.PushAsync(new LichSuPage(4));
            });
        }
    }
}