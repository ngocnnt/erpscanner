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
using ZXing.Common;
using Plugin.Screenshot;
using VTTBBarcode.Views;

namespace VTTBBarcode.ViewModels
{
    public class NhapKhoViewModel : BaseViewModel
    {
        Page2 _myModalPage;
        public Command ScanCommand { get; }
        public Command SaveCommand { get; }
        public Command DataCommand { get; }
        public Command AcceptCommand { get; }
        public Command DeclineCommand { get; }
        public Command ExportCommand { get; }
        public SfDataGrid sfDataGrid { get; set; }
        public Command BackCommand { get; }

        string _qRCodeInfo;
        public string QRCodeInfo { get => _qRCodeInfo; set => SetProperty(ref _qRCodeInfo, value); }
        string QRCodeDataTen = "";
        string QRCodeDataMahieu = "";
        string QRCodeDataMaGop = "";
        string QRCodeDataNhaSX = "";
        string QRCodeDataSoPhieu = "";

        //private DataHandler dataAccess;
        string _maCode;
        public string MaCode { get => _maCode; set => SetProperty(ref _maCode, value); }
        string _maVTTB;
        public string MaVTTB { get => _maVTTB; set => SetProperty(ref _maVTTB, value); }
        ObservableCollection<NhapKhoTable> _listTemp;
        ObservableCollection<NhapKhoTable> _nKTable;
        public ObservableCollection<NhapKhoTable> NKTable
        {
            get { return _nKTable; }
            set
            {
                _nKTable = value;
                OnPropertyChanged("NKTable");
            }
        }

        ObservableCollection<DanhSachKho> _listKhoNhap;
        public ObservableCollection<DanhSachKho> ListKhoNhap
        {
            get { return _listKhoNhap; }
            set
            {
                _listKhoNhap = value;
                OnPropertyChanged("ListKhoNhap");
            }
        }
        public DanhSachKho _selectedKho;
        public DanhSachKho SelectedKho
        {
            get => _selectedKho;
            set => SetProperty(ref _selectedKho, value);
        }

        string _selectedHTNK;
        public string SelectedHTNK { get => _selectedHTNK; set => SetProperty(ref _selectedHTNK, value); }
        string _selectedCLVTTB;
        public string SelectedCLVTTB { get => _selectedCLVTTB; set => SetProperty(ref _selectedCLVTTB, value); }

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
        ObservableCollection<NhapKhoTableERP> _nKTableERP;
        public ObservableCollection<NhapKhoTableERP> NKTableERP
        {
            get { return _nKTableERP; }
            set
            {
                _nKTableERP = value;
                OnPropertyChanged("NKTableERP");
            }
        }
        //CMIS
        public SfDataGrid sfDataGridCMIS { get; set; }
        ObservableCollection<NhapKhoTableCMIS> _nKTableCMIS;
        public ObservableCollection<NhapKhoTableCMIS> NKTableCMIS
        {
            get { return _nKTableCMIS; }
            set
            {
                _nKTableCMIS = value;
                OnPropertyChanged("NKTableCMIS");
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

        public NhapKhoViewModel()
        {
            Title = "Nhập kho";
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
            BackCommand = new Command(OnBackCLicked);

            LoadData();
            ListKhoNhap = listKho;
        }
        bool CanExecuteDataCommand(object arg)
        {
            if ((NKTable == null) || (NKTable.Count == 0))
                return false;
            else return true;
        }

        private async void OnScanCLicked(object obj)
        {
            if (NKTable.Count >= 20)
            {
                DependencyService.Get<IToast>().Show("Lô thực hiện không được nhiều hơn 20 thiết bị!");
                return;
            }
            if (SelectedKho == null || SelectedHTNK == null || SelectedCLVTTB == null)
            {
                DependencyService.Get<IToast>().Show("Vui lòng chọn các thông tin nhập kho trước khi thực hiện quét!");
                return;
            }
            IsOpenPopupThung = false;
            _chophepTH = true;
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
                if (result == null) return;  //user bấm back
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
                //if (format.ToUpper() == "QR_CODE" || format.ToUpper() == "QRCODE")
                if (result.Contains("-") || result.Contains("_"))
                {
                    //lấy ds cto từ thùng QRcode
                    if (result.Contains("-"))
                    {
                        var dataT = new
                        {
                            cloai = result,
                        };
                        var httpContentT = new StringContent(JsonConvert.SerializeObject(dataT), Encoding.UTF8, "application/json");
                        var responseT = await client.PostAsync(UrlThung + "barcode_check_sothung", httpContentT);
                        var responseContentT = responseT.Content.ReadAsStringAsync().Result;
                        data = responseContentT;
                        data = data.Replace("\"", "");
                    }
                    else
                    {
                        data = result.Replace("_", ",");
                    }
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
                //if (format.ToUpper() == "QR_CODE" || format.ToUpper() == "QRCODE")
                if (result.Contains("-") || result.Contains("_"))
                {
                    DSThung = contents;
                    foreach (CongTo ct in DSThung)
                    {
                        if ((ct.vttB_Status != TThai03) && (ct.vttB_Status != TThai05) && (ct.vttB_Status != TThai00))
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial ở trạng thái không cho phép thực hiện '{0}'. Anh/ chị vui lòng kiểm tra lại!", Title));
                            _chophepTH = false;
                            return;
                        }
                        if (((ct.vttB_Status == TThai03) && (SelectedHTNK != "Nhập kho thu hồi"))
                            || ((ct.vttB_Status == TThai05) && (SelectedHTNK != "Nhập kho sau gia công, sửa chữa, thí nghiệm") && (SelectedHTNK != "Nhập kho sau bảo hành"))
                            || ((ct.vttB_Status == TThai00) && (SelectedHTNK != "Nhập mua ngoài")))
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial ở trạng thái không cho phép thực hiện '{0}'. Anh/ chị vui lòng kiểm tra lại!", SelectedHTNK));
                            _chophepTH = false;
                            return;
                        }
                        //tình trạng kđ
                        if (((ct.checkedResult == null) && (SelectedCLVTTB == "C70"))
                            || ((ct.checkedResult == true) && (SelectedCLVTTB == "A70"))
                            || ((ct.checkedResult == false) && (SelectedCLVTTB == "D50"))
                            || ((ct.checkedResult == true) && (SelectedCLVTTB == "000") && (ct.vttB_Status == TThai00)))
                        {
                        }
                        else
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial ở trạng thái kiểm định không phù hợp với Chất lượng VTTB đã chọn. Anh/ chị vui lòng kiểm tra lại!"));
                            _chophepTH = false;
                            return;
                        }
                    }
                    IsOpenPopupThung = true;
                }
                else
                {
                    MaVTTB = "3.60.05.130.VIE.SE." + SelectedCLVTTB;
                    if ((contents[0].vttB_Status != TThai03) && (contents[0].vttB_Status != TThai05) && (contents[0].vttB_Status != TThai00))
                    {
                        DependencyService.Get<IToast>().Show(string.Format("Trạng thái serial {0}: {1}, không cho phép thực hiện '{2}'. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, Title));
                        return;
                    }
                    if (((contents[0].vttB_Status == TThai03) && (SelectedHTNK != "Nhập kho thu hồi"))
                            || ((contents[0].vttB_Status == TThai05) && (SelectedHTNK != "Nhập kho sau gia công, sửa chữa, thí nghiệm") && (SelectedHTNK != "Nhập kho sau bảo hành"))
                            || ((contents[0].vttB_Status == TThai00) && (SelectedHTNK != "Nhập mua ngoài")))
                    {
                        DependencyService.Get<IToast>().Show(string.Format("Trạng thái serial {0}: {1} không cho phép thực hiện '{2}'. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, SelectedHTNK));
                        _chophepTH = false;
                        return;
                    }
                    //tình trạng kđ
                    if (((contents[0].checkedResult == null) && (SelectedCLVTTB == "C70"))
                        || ((contents[0].checkedResult == true) && (SelectedCLVTTB == "A70"))
                        || ((contents[0].checkedResult == false) && (SelectedCLVTTB == "D50"))
                        || ((contents[0].checkedResult == true) && (SelectedCLVTTB == "000") && (contents[0].vttB_Status == TThai00)))
                    {
                    }
                    else
                    {
                        DependencyService.Get<IToast>().Show(string.Format("Serial {0} ở trạng thái kiểm định: {1} không phù hợp với Chất lượng VTTB đã chọn. Anh/ chị vui lòng kiểm tra lại!", data, (contents[0].checkedResult == true) ? "Đạt" : (contents[0].checkedResult == false) ? "Không đạt" : "Chưa kiểm định"));
                        _chophepTH = false;
                        return;
                    }

                    bool itemExists = NKTable.Any(item =>
                    {
                        return (item.MaCode == result) &&
                               (item.User == UserName);
                    });
                    if (!itemExists)
                    {
                        s++;
                        NhapKhoTable dt = new NhapKhoTable();
                        dt.User = UserName;
                        dt.MaCode = result;
                        dt.vttB_Status = contents[0].vttB_Status;
                        dt.MaVTTB = MaVTTB;
                        dt.KhoGD = SelectedKho.MAKHO;
                        dt.HinhThucNK = SelectedHTNK;
                        dt.ChatLgVTTB = SelectedCLVTTB;
                        dt.NgayXL = DateTime.Now;
                        dt.STT = s;
                        dt.CLoai = contents[0].code_CLoai != null ? contents[0].code_CLoai : (contents[0].code == "DT01P-RF" ? "D43" : "");
                        dt.NamSX = contents[0].namSX;
                        dt.CheckedInfo = contents[0].checkedInfo;
                        dt.CheckedEXDate = (contents[0].checkedEXDate != null) ? contents[0].checkedEXDate.Value.ToString("dd/MM/yyyy") : "";
                        dt.CheckedResult = contents[0].checkedResult;
                        dt.Ten_Dviqly = contents[0].ten_Dviqly;
                        dt.Ma_ChiKD = contents[0].ma_ChiKD;
                        dt.Ma_NvienKD = contents[0].ma_NvienKD;
                        NKTable.Add(dt);
                        DataCommand?.ChangeCanExecute();
                        if (NKTable.Count == 1) //lưu ở record đầu
                        {
                            QRCodeDataTen = contents[0].descriptionName;
                            QRCodeDataMahieu = contents[0].code;
                            QRCodeDataMaGop = contents[0].code_Chung;
                            QRCodeDataNhaSX = contents[0].nhaSX;
                        }
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
        
        private async void OnAcceptCLicked(object obj)
        {
            MaVTTB = "3.60.05.130.VIE.SE." + SelectedCLVTTB;
            if (_chophepTH)
            {
                foreach (CongTo ct in DSThung)
                {
                    bool itemExists = NKTable.Any(item =>
                    {
                        return (item.MaCode == ct.serialNum) &&
                               (item.User == UserName);
                    });
                    if (!itemExists)
                    {
                        s++;
                        NhapKhoTable ds = new NhapKhoTable();
                        ds.User = UserName;
                        ds.MaCode = ct.serialNum;
                        ds.vttB_Status = ct.vttB_Status;
                        ds.MaVTTB = MaVTTB;
                        ds.KhoGD = SelectedKho.MAKHO;
                        ds.HinhThucNK = SelectedHTNK;
                        ds.ChatLgVTTB = SelectedCLVTTB;
                        ds.NgayXL = DateTime.Now;
                        ds.STT = s;
                        ds.CLoai = ct.code_CLoai != null ? ct.code_CLoai : (ct.code == "DT01P-RF" ? "D43" : "");
                        ds.NamSX = ct.namSX;
                        ds.CheckedInfo = ct.checkedInfo;
                        ds.CheckedEXDate = (ct.checkedEXDate != null) ? ct.checkedEXDate.Value.ToString("dd/MM/yyyy") : "";
                        ds.CheckedResult = ct.checkedResult;
                        ds.Ten_Dviqly = ct.ten_Dviqly;
                        ds.Ma_ChiKD = ct.ma_ChiKD;
                        ds.Ma_NvienKD = ct.ma_NvienKD;
                        NKTable.Add(ds);
                        DataCommand?.ChangeCanExecute();
                        if (NKTable.Count == 1) //lưu ở record đầu
                        {
                            QRCodeDataTen = ct.descriptionName;
                            QRCodeDataMahieu = ct.code;
                            QRCodeDataMaGop = ct.code_Chung;
                            QRCodeDataNhaSX = ct.nhaSX;
                        }
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
            _listTemp = new ObservableCollection<NhapKhoTable>(dataAccess.LoadRecordNhapKho(UserName, 0));
            foreach (NhapKhoTable t in _listTemp)
            {
                s++;
                t.STT = s;
            }
            NKTable = _listTemp;
            DataCommand?.ChangeCanExecute();
            QRCodeDataTen = "";
            QRCodeDataMahieu = "";
            QRCodeDataMaGop = "";
            QRCodeDataNhaSX = "";
            QRCodeDataSoPhieu = "";
        }

        private void OnSaveCLicked(object obj)
        {
            if ((NKTable == null) || (NKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để lưu. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                foreach (NhapKhoTable ma in NKTable)
                {
                    if (ma != null)
                    {
                        dataAccess.SaveRecordNhapKho(ma);
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
                dataAccess.DeleteNhapKho(UserName);
                LoadData();
            }

            catch (Exception ex)
            {
                HideLoading();
            }
        }
        private async void OnExportCLicked(object obj)
        {
            if ((NKTable == null) || (NKTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để xuất excel. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                try
                {
                    //lấy thông tin
                    var ok = await new MessageXacThuc("NK", NKTable[0].KhoGD).Show();
                    if (ok == DialogReturn.Cancel)
                    {
                        return;
                    }
                    //tạo table erp và cmis
                    ShowLoading("Vui lòng đợi");
                    await Task.Delay(200);
                    exportERP();
                    if (SelectedCLVTTB == "A70")
                        exportCMIS();
                    if (QRCodeDataTen == "")
                    {
                        var response = await client.GetAsync(Url + "CongTos/" + NKTable[0].MaCode);
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        ObservableCollection<CongTo> contents = JsonConvert.DeserializeObject<ObservableCollection<CongTo>>(responseContent);
                        QRCodeDataTen = contents[0].descriptionName;
                        QRCodeDataMahieu = contents[0].code;
                        QRCodeDataMaGop = contents[0].code_Chung;
                        QRCodeDataNhaSX = contents[0].nhaSX;
                    }
                    HideLoading();
                    DependencyService.Get<IToast>().Show("Xuất excel thành công!");
                    //show mã QRCode lên mh
                    QRCodeInfo = "Tên mô tả VTTB: " + QRCodeDataTen + Environment.NewLine;
                    QRCodeInfo += "Mã hiệu hàng hóa: " + QRCodeDataMahieu + Environment.NewLine;
                    QRCodeInfo += "Mã VTTB | Kiểu | Mã CL: " + QRCodeDataMaGop + Environment.NewLine;
                    QRCodeInfo += "Nhà sản xuất: " + QRCodeDataNhaSX + Environment.NewLine;
                    QRCodeInfo += "Số phiếu giao dịch: " + QRCodeDataSoPhieu + Environment.NewLine;
                    QRCodeInfo += "Danh sách số chế tạo: " + dsSerial.Replace("_", "; ") + Environment.NewLine;
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
                //await DependencyService.Get<ISave>().SaveAndView("NhapKho_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", "application/msexcel", stream, "");
                //DependencyService.Get<IToast>().Show("Xuất excel thành công!");
            }
        }
        ObservableCollection<ObservableCollection<NhapKhoTable>> Split(ObservableCollection<NhapKhoTable> collection, int splitBy)
        {
            var result = collection
                       .Select((x, i) => new { index = i, item = x })
                       .GroupBy(x => x.index / splitBy, x => x.item)
                       .Select(g => new ObservableCollection<NhapKhoTable>(g));
            return new ObservableCollection<ObservableCollection<NhapKhoTable>>(result);
        }

        private string getCodeBDong(string bd)
        {
            switch (bd)
            {
                case "Nhập mua ngoài":
                    return "33";
                case "Nhập kho thu hồi":
                    return "45";       
                case "Nhập kho sau gia công, sửa chữa, thí nghiệm":
                    return "333";
                case "Nhập kho sau bảo hành":
                    return "332";
                default:
                    return "";
            }
        }

        public async void exportERP()
        {
            ObservableCollection<ObservableCollection<NhapKhoTable>> groupSplit = new ObservableCollection<ObservableCollection<NhapKhoTable>>();
            groupSplit = Split(NKTable, NKTable.Count); //Split(NKTable, 10);
            for (int j = 0; j < groupSplit.Count; j++)
            {
                NKTableERP = new ObservableCollection<NhapKhoTableERP>();
                int index = 0;
                if (groupSplit[j].Count == 1)
                {
                    NhapKhoTableERP ds = new NhapKhoTableERP();
                    ds.Column1 = @"\{ENTER}";
                    ds.Column2 = @"*SL0,5";
                    ds.Column3 = @"\+{TAB}";
                    ds.Column4 = "%" + groupSplit[j][0].KhoGD + "%";
                    ds.Column5 = @"\{ENTER}";
                    ds.Column6 = @"\{DOWN}";
                    ds.Column7 = @"\{ENTER}";
                    ds.Column8 = "*SL0,5";
                    ds.Column9 = DateTime.Now.ToString();
                    ds.Column10 = @"\{TAB}";
                    ds.Column11 = "Account alias Receipt";
                    ds.Column12 = @"\{TAB}";
                    ds.Column13 = getReasonCodeD(getCodeBDong(groupSplit[j][0].HinhThucNK));
                    ds.Column14 = @"\{TAB}";
                    ds.Column15 = getReasonCodeC(getCodeBDong(groupSplit[j][0].HinhThucNK));
                    ds.Column16 = @"\{TAB 2}";
                    ds.Column17 = @"\^L";
                    ds.Column18 = @"\%O";
                    ds.Column19 = @"\{TAB}";
                    ds.Column20 = InfoPopup3NK;
                    ds.Column21 = @"\{TAB 4}";
                    ds.Column22 = "Receipt Transaction Info.";
                    ds.Column23 = @"\{TAB}";
                    ds.Column24 = InfoPopup1NK;
                    ds.Column25 = @"\{TAB 2}";
                    ds.Column26 = InfoPopup2NK;
                    ds.Column27 = @"\{TAB 3}";
                    ds.Column28 = InfoPopup6NK;
                    ds.Column29 = @"\{TAB 4}";
                    ds.Column30 = InfoPopup5NK;
                    ds.Column31 = @"\%O";
                    ds.Column32 = @"\%R";
                    ds.Column33 = "*SL0,5";
                    ds.Column34 = "3.60.05.130.VIE.SE." + groupSplit[j][0].ChatLgVTTB;
                    ds.Column35 = @"\{TAB 2}";
                    ds.Column36 = InfoPopup4NK;
                    ds.Column37 = @"\{TAB 3}";
                    ds.Column38 = NKTable.Count.ToString();
                    ds.Column39 = @"\{TAB}";
                    ds.Column40 = InfoPopup7NK;
                    ds.Column41 = @"\%R";
                    ds.Column42 = "*SL0,5";
                    ds.Column43 = @"\{TAB}";
                    ds.Column44 = @"\%O";
                    ds.Column45 = @"\%I";
                    ds.Column46 = groupSplit[j][0].MaCode;
                    ds.Column47 = @"\{TAB}";
                    ds.Column48 = "";
                    ds.Column49 = "";
                    ds.Column50 = @"\%D";
                    ds.Column51 = "*SL0,5";
                    ds.Column52 = @"\^S";
                    ds.Column53 = @"\%O";
                    ds.Column54 = @"\^{F4}";
                    ds.Column55 = @"\{UP}";
                    ds.Column56 = "*SL3";
                    NKTableERP.Add(ds);
                }
                else foreach (NhapKhoTable ct in groupSplit[j])
                {
                    if (index == 0)
                    {
                        NhapKhoTableERP ds = new NhapKhoTableERP();
                        ds.Column1 = @"\{ENTER}";
                        ds.Column2 = @"*SL0,5";
                        ds.Column3 = @"\+{TAB}";
                        ds.Column4 = "%" + ct.KhoGD + "%";
                        ds.Column5 = @"\{ENTER}";
                        ds.Column6 = @"\{DOWN}";
                        ds.Column7 = @"\{ENTER}";
                        ds.Column8 = "*SL0,5";
                        ds.Column9 = DateTime.Now.ToString();
                        ds.Column10 = @"\{TAB}";
                        ds.Column11 = "Account alias Receipt";
                        ds.Column12 = @"\{TAB}";
                        ds.Column13 = getReasonCodeD(getCodeBDong(ct.HinhThucNK));
                        ds.Column14 = @"\{TAB}";
                        ds.Column15 = getReasonCodeC(getCodeBDong(ct.HinhThucNK));
                        ds.Column16 = @"\{TAB 2}";
                        ds.Column17 = @"\^L";
                        ds.Column18 = @"\%O";
                        ds.Column19 = @"\{TAB}";
                        ds.Column20 = InfoPopup3NK;
                        ds.Column21 = @"\{TAB 4}";
                        ds.Column22 = "Receipt Transaction Info.";
                        ds.Column23 = @"\{TAB}";
                        ds.Column24 = InfoPopup1NK;
                        ds.Column25 = @"\{TAB 2}";
                        ds.Column26 = InfoPopup2NK;
                        ds.Column27 = @"\{TAB 3}";
                        ds.Column28 = InfoPopup6NK;
                        ds.Column29 = @"\{TAB 4}";
                        ds.Column30 = InfoPopup5NK;
                        ds.Column31 = @"\%O";
                        ds.Column32 = @"\%R";
                        ds.Column33 = "*SL0,5";
                        ds.Column34 = "3.60.05.130.VIE.SE." + ct.ChatLgVTTB;
                        ds.Column35 = @"\{TAB 2}";
                        ds.Column36 = InfoPopup4NK; 
                        ds.Column37 = @"\{TAB 3}";
                        ds.Column38 = NKTable.Count.ToString();
                        ds.Column39 = @"\{TAB}";
                        ds.Column40 = InfoPopup7NK;
                        ds.Column41 = @"\%R";
                        ds.Column42 = "*SL0,5";
                        ds.Column43 = @"\{TAB}";
                        ds.Column44 = @"\%O";
                        ds.Column45 = @"\%I";
                        ds.Column46 = ct.MaCode;
                        ds.Column47 = "";
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = "";
                        ds.Column51 = "";
                        ds.Column52 = @"\{DOWN}";
                        ds.Column53 = "";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = "";
                        NKTableERP.Add(ds);
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
                        ds.Column46 = ct.MaCode;
                        ds.Column47 = @"\{TAB}";
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = @"\%D";
                        ds.Column51 = "*SL0,5";
                        ds.Column52 = @"\^S";
                        ds.Column53 = @"\%O";
                        ds.Column54 = @"\^{F4}";
                        ds.Column55 = @"\{UP}";
                        ds.Column56 = "*SL3";
                        NKTableERP.Add(ds);
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
                        ds.Column46 = ct.MaCode;
                        ds.Column47 = "";
                        ds.Column48 = "";
                        ds.Column49 = "";
                        ds.Column50 = "";
                        ds.Column51 = "";
                        ds.Column52 = @"\{DOWN}";
                        ds.Column53 = "";
                        ds.Column54 = "";
                        ds.Column55 = "";
                        ds.Column56 = "";
                        NKTableERP.Add(ds);
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
                string filename = UserName + gettenfile(groupSplit[j][0].KhoGD, getCodeBDong(groupSplit[j][0].HinhThucNK), groupSplit[j][0].ChatLgVTTB) + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-ERP-" + (j + 1).ToString("00") + ".xlsx";
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
                    foreach (NhapKhoTable ct in NKTable)
                    {
                        dsSerial += ct.MaCode + "_";
                    }
                    dsSerial = dsSerial.Substring(0, dsSerial.Length - 1);
                    FileDaGoiTable ma = new FileDaGoiTable();
                    ma.ChucNang = 2;
                    ma.MaQRCode = dsSerial;
                    ma.TenFile = filename.Substring(0, filename.IndexOf("-ERP-", StringComparison.Ordinal));
                    ma.User = UserName;
                    ma.NgayXL = DateTime.Now;
                    idFile = dataAccess.SaveRecordFile(ma);
                    foreach (NhapKhoTable ct in NKTable)
                    {
                        ct.IDFile = idFile;
                        dataAccess.SaveRecordNhapKho(ct);
                    }
                    fileNameex = ma.TenFile + ".jpg";
                    QRCodeDataSoPhieu = filename;
                }
            }
        }

        public async void exportCMIS()
        {
            NKTableCMIS = new ObservableCollection<NhapKhoTableCMIS>();
            foreach (NhapKhoTable ct in NKTable)
            {
                NhapKhoTableCMIS ds = new NhapKhoTableCMIS();
                string[] kdinhInfo = ct.CheckedInfo.Split('|');
                ds.MA_DVIQLY = userInfo.MA_DVI_QLY;
                ds.SO_BBAN_KD = (kdinhInfo != null) ? kdinhInfo[0] : "";
                ds.MA_CTO = ct.CLoai + ct.NamSX + ct.MaCode;
                ds.SO_CTO = ct.MaCode;
                ds.MA_DVIKD = getMA_DVIKD(ct.Ten_Dviqly);
                ds.MA_NVIENKD = ct.Ma_NvienKD;
                ds.NOI_DUNG = "Đạt";
                ds.SLAN_LT = "";
                ds.NGAY_LTRINH = ""; 
                ds.BCS = "KT;VC";
                ds.HSN = "1";
                ds.HS_PHU = "0";
                ds.TYSO_TI = "1";
                ds.TYSO_TU = "1";
                ds.MA_CHIKD = ct.Ma_ChiKD;
                ds.SO_CHIKD = "2";
                ds.KIM_CHITAI = ds.MA_CHIKD;
                ds.SO_CHITAI = "1";
                ds.MTEM_KD = "1";
                ds.SERY_TEMKD = (kdinhInfo != null) ? kdinhInfo[3] : "";
                ds.TINH_TRANG = "1";
                ds.KD_HTRUONG = "0";
                ds.NGAY_KDINH = (kdinhInfo != null) ? DateTime.Parse(kdinhInfo[1]).ToString("dd/MM/yyyy") : "";
                ds.HAN_KDINH = (ct.CheckedEXDate != null) ? DateTime.ParseExact(ct.CheckedEXDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                ds.THU_NGUYEN = "1";
                ds.MA_NVIENLT = "";
                ds.MTEM_CQ = "";
                ds.SERY_TEMCQ = "";
                ds.HTHUC_KD = "Q";
                NKTableCMIS.Add(ds);
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
            string filename = UserName + gettenfile(NKTable[0].KhoGD, getCodeBDong(NKTable[0].HinhThucNK), NKTable[0].ChatLgVTTB) + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-CMIS-01" + ".xlsx";
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
                //Shell.Current.Navigation.PushAsync(new LichSuPage(2));
                Shell.Current.Navigation.PushModalAsync(new LichSuPage(2));
            });
        }
        private void OnBackCLicked(object obj)
        {
            IsOpenPopupQRCode = false;
        }
    }
}