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
    public class ChuyenKhoViewModel : BaseViewModel
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
        ObservableCollection<ChuyenKhoTable> _listTemp;
        ObservableCollection<ChuyenKhoTable> _cKTable;
        public ObservableCollection<ChuyenKhoTable> CKTable
        {
            get { return _cKTable; }
            set
            {
                _cKTable = value;
                OnPropertyChanged("CKTable");
            }
        }

        ObservableCollection<DanhSachKho> _listKhoChuyen;
        public ObservableCollection<DanhSachKho> ListKhoChuyen
        {
            get { return _listKhoChuyen; }
            set
            {
                _listKhoChuyen = value;
                OnPropertyChanged("ListKhoChuyen");
            }
        }
        public DanhSachKho _selectedKho;
        public DanhSachKho SelectedKho
        {
            get => _selectedKho;
            set => SetProperty(ref _selectedKho, value);
        }
        string _khoTon;
        public string KhoTon { get => _khoTon; set => SetProperty(ref _khoTon, value); }
        string _selectedHTCK;
        public string SelectedHTCK { get => _selectedHTCK; set => SetProperty(ref _selectedHTCK, value); }

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
        ObservableCollection<NhapKhoTableERP> _cKTableERP;
        public ObservableCollection<NhapKhoTableERP> CKTableERP
        {
            get { return _cKTableERP; }
            set
            {
                _cKTableERP = value;
                OnPropertyChanged("CKTableERP");
            }
        }
        //CMIS
        public SfDataGrid sfDataGridCMIS { get; set; }
        ObservableCollection<ChuyenKhoTableCMIS> _cKTableCMIS;
        public ObservableCollection<ChuyenKhoTableCMIS> CKTableCMIS
        {
            get { return _cKTableCMIS; }
            set
            {
                _cKTableCMIS = value;
                OnPropertyChanged("CKTableCMIS");
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

        public ChuyenKhoViewModel()
        {
            Title = "Chuyển kho";
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
            ListKhoChuyen = listKho;
        }
        bool CanExecuteDataCommand(object arg)
        {
            if ((CKTable == null) || (CKTable.Count == 0))
                return false;
            else return true;
        }

        private async void OnScanCLicked(object obj)
        {
            if (CKTable.Count >= 20)
            {
                DependencyService.Get<IToast>().Show("Lô thực hiện không được nhiều hơn 20 thiết bị!");
                return;
            }
            if (SelectedKho == null || SelectedHTCK == null)
            {
                DependencyService.Get<IToast>().Show("Vui lòng chọn các thông tin chuyển kho trước khi thực hiện quét!");
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
                            if (ct.kho == SelectedKho.MAKHO)
                            {
                                DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial đang ở kho trùng với kho chuyển đến. Anh/ chị vui lòng kiểm tra lại!", Title));
                                _chophepTH = false;
                                return;
                            }
                        }
                        IsOpenPopupThung = true;
                    }
                    else
                    {
                        MaVTTB = "3.60.05.130.VIE.SE." + ((contents[0].checkedResult == true) ? "A70" : ((contents[0].checkedResult == false) ? "D50" : "C70"));
                        KhoTon = contents[0].ten_Kho;
                        if (contents[0].vttB_Status != TThai02)
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trạng thái serial {0}: {1}, không cho phép thực hiện '{2}'. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, Title));
                            return;
                        }
                        if (contents[0].kho == SelectedKho.MAKHO)
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Kho chuyển đến không được trùng với kho đang tồn. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, Title));
                            return;
                        }
                        bool itemExists = CKTable.Any(item =>
                        {
                            return (item.MaCode == result.Text) &&
                                   (item.User == UserName);
                        });
                        if (!itemExists)
                        {
                            s++;
                            ChuyenKhoTable dt = new ChuyenKhoTable();
                            dt.User = UserName;
                            dt.MaCode = result.Text;
                            dt.vttB_Status = contents[0].vttB_Status;
                            dt.MaVTTB = MaVTTB;
                            dt.HinhThucCK = SelectedHTCK;
                            dt.KhoTon = contents[0].kho;
                            dt.KhoTonPhu = contents[0].kho_Phu;
                            dt.KhoChuyen = SelectedKho.MAKHO;
                            dt.NgayXL = DateTime.Now;
                            dt.STT = s;
                            dt.CLoai = contents[0].code_CLoai != null ? contents[0].code_CLoai : (contents[0].code == "DT01P-RF" ? "D43" : "");
                            dt.NamSX = contents[0].namSX;
                            dt.CheckedInfo = contents[0].checkedInfo;
                            dt.CheckedEXDate = (contents[0].checkedEXDate != null) ? contents[0].checkedEXDate.Value.ToString("dd/MM/yyyy") : "";
                            dt.CheckedResult = contents[0].checkedResult;
                            CKTable.Add(dt);
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
            MaVTTB = "3.60.05.130.VIE.SE." + ((DSThung[0].checkedResult == true) ? "A70" : ((DSThung[0].checkedResult == false) ? "D50" : "C70"));
            KhoTon = DSThung[0].ten_Kho;
            if (_chophepTH)
            {
                foreach (CongTo ct in DSThung)
                {                    
                    bool itemExists = CKTable.Any(item =>
                    {
                        return (item.MaCode == ct.serialNum) &&
                               (item.User == UserName);
                    });
                    if (!itemExists)
                    {
                        s++;
                        ChuyenKhoTable ds = new ChuyenKhoTable();
                        ds.User = UserName;
                        ds.MaCode = ct.serialNum;
                        ds.vttB_Status = ct.vttB_Status;
                        ds.MaVTTB = MaVTTB;
                        ds.HinhThucCK = SelectedHTCK;
                        ds.KhoTon = ct.kho;
                        ds.KhoTonPhu = ct.kho_Phu;
                        ds.KhoChuyen = SelectedKho.MAKHO;
                        ds.NgayXL = DateTime.Now;
                        ds.STT = s;
                        ds.CLoai = ct.code_CLoai != null ? ct.code_CLoai : (ct.code == "DT01P-RF" ? "D43" : "");
                        ds.NamSX = ct.namSX;
                        ds.CheckedInfo = ct.checkedInfo;
                        ds.CheckedEXDate = (ct.checkedEXDate != null) ? ct.checkedEXDate.Value.ToString("dd/MM/yyyy") : "";
                        ds.CheckedResult = ct.checkedResult;
                        CKTable.Add(ds);
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
            _listTemp = new ObservableCollection<ChuyenKhoTable>(dataAccess.LoadRecordChuyenKho(UserName, 0));
            foreach (ChuyenKhoTable t in _listTemp)
            {
                s++;
                t.STT = s;
            }
            CKTable = _listTemp;
            DataCommand?.ChangeCanExecute();
        }

        private void OnSaveCLicked(object obj)
        {
            if ((CKTable == null) || (CKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để lưu. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                foreach (ChuyenKhoTable ma in CKTable)
                {
                    if (ma != null)
                    {
                        dataAccess.SaveRecordChuyenKho(ma);
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
                dataAccess.DeleteChuyenKho(UserName);
                LoadData();
            }

            catch (Exception ex)
            {
                HideLoading();
            }
        }
        private async void OnExportCLicked(object obj)
        {
            if ((CKTable == null) || (CKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để xuất excel. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                try
                {
                    //lấy thông tin
                    var ok = await new MessageXacThuc("CK", CKTable[0].KhoTon).Show();
                    if (ok == DialogReturn.OK)
                    {
                        //Preferences.Set(Config.AprroveFinger, _toggledVanTay);
                    }
                    //tạo table erp và cmis
                    ShowLoading("Vui lòng đợi");
                    await Task.Delay(200);
                    exportERP();
                    exportCMIS();
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
        private string getCodeBDong(string bd)
        {
            switch (bd)
            {
                case "Chuyển kho nội bộ (Cấp 3 thực hiện)":
                    return "35";
                case "Chuyển kho nội bộ (Cấp 4 thực hiện)":
                    return "46";
                default:
                    return "";
            }
        }
        public async void exportERP()
        {
            ObservableCollection<ObservableCollection<ChuyenKhoTable>> groupSplit = new ObservableCollection<ObservableCollection<ChuyenKhoTable>>();
            groupSplit = Split(CKTable);
            for (int j = 0; j < groupSplit.Count; j++)
            {
                CKTableERP = new ObservableCollection<NhapKhoTableERP>();
                int index = 0;
                if (groupSplit[j].Count == 1)
                {
                    NhapKhoTableERP ds = new NhapKhoTableERP();
                    ds.Column1 = @"\{ENTER}";
                    ds.Column2 = @"*SL0,5";
                    ds.Column3 = @"\+{TAB}";
                    ds.Column4 = "%" + groupSplit[j][0].KhoTon + "%";
                    ds.Column5 = @"\{ENTER}";
                    ds.Column6 = @"\{DOWN 2}";
                    ds.Column7 = @"\{ENTER}";
                    ds.Column8 = "*SL0,5";
                    ds.Column9 = DateTime.Now.ToString();
                    ds.Column10 = @"\{TAB}";
                    ds.Column11 = groupSplit[j][0].KhoChuyen;
                    ds.Column12 = @"\{TAB}";
                    ds.Column13 = "Direct Org Transfer";
                    ds.Column14 = @"\{TAB 2}";
                    ds.Column15 = "56";
                    ds.Column16 = @"\{TAB}";
                    ds.Column17 = @"\^L";
                    ds.Column18 = @"\%O";
                    ds.Column19 = @"\{TAB}";
                    ds.Column20 = InfoPopup3;
                    ds.Column21 = @"\{TAB 7}";
                    ds.Column22 = "*SL0,5";
                    ds.Column23 = @"\{TAB 2}";
                    ds.Column24 = "Direct Transaction Informations";
                    ds.Column25 = @"\{TAB}";
                    ds.Column26 = InfoPopup1;
                    ds.Column27 = @"\{TAB 5}";
                    ds.Column28 = InfoPopup6;
                    ds.Column29 = @"\{TAB}";
                    ds.Column30 = InfoPopup5;
                    ds.Column31 = @"\%O";
                    ds.Column32 = @"\%R";
                    ds.Column33 = "*SL0,5";
                    ds.Column34 = groupSplit[j][0].MaVTTB; //"3.60.05.130.VIE.SE." + ((ct.CheckedResult == true) ? "A70" : ((ct.CheckedResult == false) ? "D50" : "C70"));
                    ds.Column35 = @"\{TAB}";
                    ds.Column36 = groupSplit[j][0].KhoTonPhu;
                    ds.Column37 = @"\{TAB 3}";
                    ds.Column38 = InfoPopup4;
                    ds.Column39 = @"\{TAB 2}";
                    ds.Column40 = CKTable.Count.ToString();
                    ds.Column41 = @"\{TAB}";
                    ds.Column42 = @"\%R";
                    ds.Column43 = "*SL0,5";
                    ds.Column44 = @"\{TAB}";
                    ds.Column45 = @"\%O";
                    ds.Column46 = @"\%I";
                    ds.Column47 = groupSplit[j][0].MaCode;
                    ds.Column48 = @"\{TAB}";
                    ds.Column49 = "";
                    ds.Column50 = "";
                    ds.Column51 = @"\%D";
                    ds.Column52 = "*SL0,5";
                    ds.Column53 = @"\^S";
                    ds.Column54 = @"\%O";
                    ds.Column55 = @"\^{F4}";
                    ds.Column56 = @"\{UP 2}";
                    ds.Column57 = "*SL3";
                    CKTableERP.Add(ds);
                }
                else foreach (ChuyenKhoTable ct in groupSplit[j])
                {
                    if (index == 0)
                    {
                        NhapKhoTableERP ds = new NhapKhoTableERP();
                        ds.Column1 = @"\{ENTER}";
                        ds.Column2 = @"*SL0,5";
                        ds.Column3 = @"\+{TAB}";
                        ds.Column4 = "%" + ct.KhoTon + "%";
                        ds.Column5 = @"\{ENTER}";
                        ds.Column6 = @"\{DOWN 2}";
                        ds.Column7 = @"\{ENTER}";
                        ds.Column8 = "*SL0,5";
                        ds.Column9 = DateTime.Now.ToString();
                        ds.Column10 = @"\{TAB}";
                        ds.Column11 = ct.KhoChuyen;
                        ds.Column12 = @"\{TAB}";
                        ds.Column13 = "Direct Org Transfer";
                        ds.Column14 = @"\{TAB 2}";
                        ds.Column15 = "56";
                        ds.Column16 = @"\{TAB}";
                        ds.Column17 = @"\^L";
                        ds.Column18 = @"\%O";
                        ds.Column19 = @"\{TAB}";
                        ds.Column20 = InfoPopup3;
                        ds.Column21 = @"\{TAB 7}";
                        ds.Column22 = "*SL0,5";
                        ds.Column23 = @"\{TAB 2}";
                        ds.Column24 = "Direct Transaction Informations";
                        ds.Column25 = @"\{TAB}";
                        ds.Column26 = InfoPopup1;
                        ds.Column27 = @"\{TAB 5}";
                        ds.Column28 = InfoPopup6;
                        ds.Column29 = @"\{TAB}";
                        ds.Column30 = InfoPopup5;
                        ds.Column31 = @"\%O";
                        ds.Column32 = @"\%R";
                        ds.Column33 = "*SL0,5";
                        ds.Column34 = ct.MaVTTB; //"3.60.05.130.VIE.SE." + ((ct.CheckedResult == true) ? "A70" : ((ct.CheckedResult == false) ? "D50" : "C70"));
                        ds.Column35 = @"\{TAB}";
                        ds.Column36 = ct.KhoTonPhu;
                        ds.Column37 = @"\{TAB 3}";
                        ds.Column38 = InfoPopup4;
                        ds.Column39 = @"\{TAB 2}";
                        ds.Column40 = CKTable.Count.ToString();
                        ds.Column41 = @"\{TAB}";
                        ds.Column42 = @"\%R";
                        ds.Column43 = "*SL0,5";
                        ds.Column44 = @"\{TAB}";
                        ds.Column45 = @"\%O";
                        ds.Column46 = @"\%I";
                        ds.Column47 = ct.MaCode;
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = "";
                        ds.Column51 = "";
                        ds.Column52 = "";
                        ds.Column53 = @"\{DOWN}";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = "";
                        ds.Column57 = "";
                        CKTableERP.Add(ds);
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
                        ds.Column47 = ct.MaCode;
                        ds.Column48 = @"\{TAB}";
                        ds.Column49 = "";
                        ds.Column50 = "";
                        ds.Column51 = @"\%D";
                        ds.Column52 = "*SL0,5";
                        ds.Column53 = @"\^S";
                        ds.Column54 = @"\%O";
                        ds.Column55 = @"\^{F4}";
                        ds.Column56 = @"\{UP 2}";
                        ds.Column57 = "*SL3";
                        CKTableERP.Add(ds);
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
                        ds.Column47 = ct.MaCode;
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = "";
                        ds.Column51 = "";
                        ds.Column52 = "";
                        ds.Column53 = @"\{DOWN}";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = "";
                        ds.Column57 = "";
                        CKTableERP.Add(ds);
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
                string filename = UserName + gettenfile(groupSplit[j][0].KhoTon, getCodeBDong(groupSplit[j][0].HinhThucCK), "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-ERP-" + (j + 1).ToString("00") + ".xlsx";
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
                    foreach (ChuyenKhoTable ct in CKTable)
                    {
                        dsSerial += ct.MaCode + "_";
                    }
                    dsSerial = dsSerial.Substring(0, dsSerial.Length - 1);
                    FileDaGoiTable ma = new FileDaGoiTable();
                    ma.ChucNang = 3;
                    ma.MaQRCode = dsSerial;
                    ma.TenFile = filename.Substring(0, filename.IndexOf("-ERP-", StringComparison.Ordinal));
                    ma.User = UserName;
                    ma.NgayXL = DateTime.Now;
                    idFile = dataAccess.SaveRecordFile(ma);
                    foreach (ChuyenKhoTable ct in CKTable)
                    {
                        ct.IDFile = idFile;
                        dataAccess.SaveRecordChuyenKho(ct);
                    }
                    fileNameex = ma.TenFile + ".jpg";
                }
            }
        }

        public async void exportCMIS()
        {
            CKTableCMIS = new ObservableCollection<ChuyenKhoTableCMIS>();
            foreach (ChuyenKhoTable ct in CKTable)
            {
                ChuyenKhoTableCMIS ds = new ChuyenKhoTableCMIS();
                ds.MA_CTO = ct.CLoai + ct.NamSX + ct.MaCode;
                CKTableCMIS.Add(ds);
            }
            //xuất excel
            StoragePermission.Check();
            DataGridExcelExportingController excelExport = new DataGridExcelExportingController();
            var excelEngine = excelExport.ExportToExcel(this.sfDataGridCMIS);
            var workbook = excelEngine.Excel.Workbooks[0];
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            workbook.Close();
            excelEngine.Dispose();
            string filename = UserName + gettenfile(CKTable[0].KhoTon, getCodeBDong(CKTable[0].HinhThucCK), "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-CMIS-01" + ".xlsx";
            await DependencyService.Get<ISave>().SaveAndView(filename, "application/msexcel", stream, "CMIS");

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
        }

        private void OnLichSuCLicked(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.Navigation.PushAsync(new LichSuPage(3));
            });
        }
    }
}