using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Screenshot;
using RestSharp;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.SfDataGrid.XForms.Exporting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using VTTBBarcode.Views;
using Xamarin.Forms;
using ZXing.Common;
using ZXing.Net.Mobile.Forms;
using ZXing.QrCode;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.ViewModels
{
    public class NhapKhoHDViewModel : BaseViewModel
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
        ObservableCollection<NhapKhoHDTable> _listTemp;
        ObservableCollection<NhapKhoHDTable> _nKHDTable;
        public ObservableCollection<NhapKhoHDTable> NKHDTable
        {
            get { return _nKHDTable; }
            set
            {
                _nKHDTable = value;
                OnPropertyChanged("NKHDTable");
            }
        }

        ObservableCollection<DanhSachHopDong> _listHopDong;
        public ObservableCollection<DanhSachHopDong> ListHopDong
        {
            get { return _listHopDong; }
            set
            {
                _listHopDong = value;
                OnPropertyChanged("ListHopDong");
            }
        }
        public DanhSachHopDong _selectedHD;
        public DanhSachHopDong SelectedHD
        {
            get => _selectedHD;
            set => SetProperty(ref _selectedHD, value);
        }

        ObservableCollection<TT_BB> _listBBGN;
        public ObservableCollection<TT_BB> ListBBGN
        {
            get { return _listBBGN; }
            set
            {
                _listBBGN = value;
                OnPropertyChanged("ListBBGN");
            }
        }
        public TT_BB _selectedBBGN;
        public TT_BB SelectedBBGN
        {
            get => _selectedBBGN;
            set => SetProperty(ref _selectedBBGN, value);
        }

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
        bool _isAcceptSend;
        //public bool IsAcceptSend { get => _isAcceptSend; set => SetProperty(ref _isAcceptSend, value); }
        public bool IsAcceptSend
        {
            get
            {
                return _isAcceptSend;
            }
            set
            {
                _isAcceptSend = value;
                OnPropertyChanged();
            }
        }

        bool _chophepTH = true;
        int s = 0;
        int _ID_VTTB = 0;
        string _ORGANIZATION_CODE = "";
        int idFile = -1;
        string dsSerial = "";
        string fileNameex = "";
        ObservableCollection<DanhSachBBGN> dsBBGN = new ObservableCollection<DanhSachBBGN>();

        //ERP
        public SfDataGrid sfDataGridERP { get; set; }
        ObservableCollection<NhapKhoHDTableERP> _nKHDTableERP;
        public ObservableCollection<NhapKhoHDTableERP> NKHDTableERP
        {
            get { return _nKHDTableERP; }
            set
            {
                _nKHDTableERP = value;
                OnPropertyChanged("NKHDTableERP");
            }
        }
        //CMIS
        public SfDataGrid sfDataGridCMIS { get; set; }
        ObservableCollection<NhapKhoHDTableCMIS> _nKHDTableCMIS;
        public ObservableCollection<NhapKhoHDTableCMIS> NKHDTableCMIS
        {
            get { return _nKHDTableCMIS; }
            set
            {
                _nKHDTableCMIS = value;
                OnPropertyChanged("NKHDTableCMIS");
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

        public NhapKhoHDViewModel()
        {
            Title = "Nhập kho theo hợp đồng";
            MaVTTB = "3.60.05.130.VIE.SE.000";
            DSThung = new ObservableCollection<CongTo>();
            NKHDTable = new ObservableCollection<NhapKhoHDTable>();
            IsOpenPopupThung = false;
            IsAcceptSend = false;
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
            LoadDSHD();
        }
        bool CanExecuteDataCommand(object arg)
        {
            if ((NKHDTable == null) || (NKHDTable.Count == 0))
                return false;
            else return true;
        }

        private async void OnScanCLicked(object obj)
        {
            if (NKHDTable.Count >= 20)
            {
                DependencyService.Get<IToast>().Show("Lô thực hiện không được nhiều hơn 20 thiết bị!");
                return;
            }    
            if (!IsAcceptSend)
            {
                DependencyService.Get<IToast>().Show("Vui lòng chọn hợp đồng và BBGN trước khi thực hiện quét!");
                return;
            }
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            ZXingScannerView zxing = new ZXingScannerView();
            ZXing.Result result = null;
            IsOpenPopupThung = false;
            _chophepTH = true;
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
                            if (ct.vttB_Status != TThai00)
                            {
                                DependencyService.Get<IToast>().Show(string.Format("Trong thùng có serial không ở trạng thái 'Chưa có lịch sử', không cho phép thực hiện '{0}'. Anh/ chị vui lòng kiểm tra lại!", Title));
                                _chophepTH = false;
                                return;
                            }
                        }
                        IsOpenPopupThung = true;
                    }
                    else
                    {
                        if (contents[0].vttB_Status != TThai00)
                        {
                            DependencyService.Get<IToast>().Show(string.Format("Trạng thái serial {0}: {1}, không cho phép thực hiện '{2}'. Anh/ chị vui lòng kiểm tra lại!", data, contents[0].vttB_Status, Title));
                            return;
                        }
                        bool itemExists = NKHDTable.Any(item =>
                        {
                            return (item.MaCode == result.Text) &&
                                   (item.User == UserName);
                        });
                        if (!itemExists)
                        {
                            s++;
                            NhapKhoHDTable dt = new NhapKhoHDTable();
                            dt.User = UserName;
                            dt.MaCode = result.Text;
                            dt.vttB_Status = contents[0].vttB_Status;
                            dt.MaVTTB = MaVTTB;
                            dt.HopDong = SelectedHD.SO_HD;
                            dt.BBGN = SelectedBBGN.SO_BB;
                            dt.GTGT = SelectedBBGN.GTGT_SO;
                            dt.ID_VTTB = _ID_VTTB.ToString();
                            dt.MaKho = _ORGANIZATION_CODE;
                            dt.NgayXL = DateTime.Now;
                            dt.STT = s;
                            dt.CLoai = contents[0].code_CLoai != null ? contents[0].code_CLoai : (contents[0].code == "DT01P-RF" ? "D43" : "");
                            dt.NamSX = contents[0].namSX;
                            dt.CheckedInfo = contents[0].checkedInfo;
                            dt.CheckedEXDate = (contents[0].checkedEXDate != null)? contents[0].checkedEXDate.Value.ToString("dd/MM/yyyy"): "";
                            dt.CheckedResult = contents[0].checkedResult;
                            dt.Ten_Dviqly = contents[0].ten_Dviqly;
                            dt.Ma_ChiKD = contents[0].ma_ChiKD;
                            dt.Ma_NvienKD = contents[0].ma_NvienKD;
                            dt.IDFile = 0;
                            NKHDTable.Add(dt);
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
            if (_chophepTH)
            {
                foreach (CongTo ct in DSThung)
                {
                    bool itemExists = NKHDTable.Any(item =>
                    {
                        return (item.MaCode == ct.serialNum) &&
                               (item.User == UserName);
                    });
                    if (!itemExists)
                    {
                        s++;
                        NhapKhoHDTable ds = new NhapKhoHDTable();
                        ds.User = UserName;
                        ds.MaCode = ct.serialNum;
                        ds.vttB_Status = ct.vttB_Status;
                        ds.MaVTTB = MaVTTB;
                        ds.HopDong = SelectedHD.SO_HD;
                        ds.BBGN = SelectedBBGN.SO_BB;
                        ds.GTGT = SelectedBBGN.GTGT_SO;
                        ds.ID_VTTB = _ID_VTTB.ToString();
                        ds.MaKho = _ORGANIZATION_CODE;
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
                        ds.IDFile = 0;
                        NKHDTable.Add(ds);
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
        private async void LoadDSHD()
        {
            //var response = await client.GetAsync(UrlHD + String.Format("BBGN_GetDSHD?username={0}&Password={1}", UserName, PassBBGN));
            //var responseContent = response.Content.ReadAsStringAsync().Result;
            var request = new RestRequest(UrlHD + "BBGN_GetDSHD", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("username", UserName);
            request.AddParameter("Password", PassBBGN);
            RestResponse response = await clientS.ExecutePostAsync(request);
            ObservableCollection<DanhSachHopDong> contents = JsonConvert.DeserializeObject<ObservableCollection<DanhSachHopDong>>(response.Content);
            ListHopDong = contents;
        }

        public async void LoadDSBBGN()
        {
            //if (ListBBGN != null)
                //ListBBGN.Clear();
                //ngocntt 2 dòng trên, lỗi crash ios, xem lại
            //var response = await client.GetAsync(UrlHD + String.Format("BBGN_GetDS_BB_HD?HOPDONG={0}&Password={1}", SelectedHD.SO_HD, PassBBGN));
            //var responseContent = response.Content.ReadAsStringAsync().Result;
            var request = new RestRequest(UrlHD + "BBGN_GetDS_BB_HD", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("HOPDONG", SelectedHD.SO_HD);
            request.AddParameter("Password", PassBBGN);
            RestResponse response = await clientS.ExecutePostAsync(request);
            ObservableCollection<DanhSachBBGN> contents = JsonConvert.DeserializeObject<ObservableCollection<DanhSachBBGN>>(response.Content);
            dsBBGN = contents;
            ObservableCollection<TT_BB> dsbb = new ObservableCollection<TT_BB>();
            foreach (DanhSachBBGN bbgn in contents)
            {
                dsbb.Add(bbgn.TT_BB);
            }
            ListBBGN = dsbb;
        }

        public async void SelectBBGN()
        {
            if (ListBBGN.Count == 0) return;
            foreach (DanhSachBBGN ds in dsBBGN)
            {
                if (SelectedBBGN == ds.TT_BB)
                {
                    foreach (DS_VT dsvt in ds.DS_VT)
                    {
                        if (dsvt.SEGMENT11 == MaVTTB)
                        {
                            _ID_VTTB = dsvt.ID;
                            _ORGANIZATION_CODE = dsvt.ORGANIZATION_CODE;
                            IsAcceptSend = true;
                            return;
                        }
                    }
                }
            }
            IsAcceptSend = false;
            DependencyService.Get<IToast>().Show("Trong BBGN đã chọn không có vật tư cho phép thực hiện. Anh/ chị vui lòng kiểm tra lại!");
        }

        private void LoadData()
        {
            s = 0;
            _listTemp = new ObservableCollection<NhapKhoHDTable>(dataAccess.LoadRecordNhapKhoHD(UserName, 0));
            foreach (NhapKhoHDTable t in _listTemp)
            {
                s++;
                t.STT = s;
            }
            NKHDTable = _listTemp;
            DataCommand?.ChangeCanExecute();
        }

        private void OnSaveCLicked(object obj)
        {
            if ((NKHDTable == null) || (NKHDTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để lưu. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            else
            {
                foreach (NhapKhoHDTable ma in NKHDTable)
                {
                    if (ma != null)
                    {
                        dataAccess.SaveRecordNhapKhoHD(ma);
                    }
                }
                DependencyService.Get<IToast>().Show("Lưu dữ liệu thành công!");
                //LoadData();
            }
        }

        private async void OnDataCLicked(object obj)
        {
            if ((NKHDTable == null) || (NKHDTable.Count == 0))
            {
                DependencyService.Get<IToast>().Show("Chưa có danh sách mã để gởi. Anh chị vui lòng kiểm tra lại!");
                return;
            }
            try
            {
                ShowLoading("Đang gởi dữ liệu vui lòng đợi");
                await Task.Delay(200);
                //gởi xong xóa
                foreach (NhapKhoHDTable ma in NKHDTable)
                {
                    var values = new Dictionary<string, string>
                    {
                        {"username", UserName},
                        {"ID_VTTB", ma.ID_VTTB},
                        {"SO_LO", ""},
                        {"mahieusx", ""},
                        {"mavach", ""},
                        {"matb", "3.60.05.130.VIE.SE.000"},
                        {"ITEM_DESC", "Công tơ điện tử 1 pha có RF DT01P-RF"},
                        {"DVT", "Cái"},
                        {"SOLUONG", "1"},
                        {"nuoc_sx", "Vietnam"},
                        {"nhasx", "EMEC"},
                        {"SO_CTAO", ma.MaCode},
                        {"TINH_TRANG", (ma.CheckedResult == true)?"Đạt":"Không Đạt"},
                        {"BB_TN", ""},
                        {"GHI_CHU", ""},
                        {"bPassword", "BBgncpcit@2022"},
                    };
                    var response = await client.PostAsync(UrlHD + "BBGN_Insert_TBChiTiet", new FormUrlEncodedContent(values));
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    if (!responseContent.Contains("OK"))
                    {
                        HideLoading();
                        DependencyService.Get<IToast>().Show("Có lỗi xảy ra trong quá trình gởi dữ liệu. Anh chị vui lòng kiểm tra lại!");
                        return;
                    }
                }
                DependencyService.Get<IToast>().Show("Gởi dữ liệu thành công!");
                HideLoading();
                dataAccess.DeleteNhapKhoHD(UserName);
                LoadData();
            }

            catch (Exception ex)
            {
                HideLoading();
            }
        }

        private async void OnExportCLicked(object obj)
        {
            if ((NKHDTable == null) || (NKHDTable.Count == 0))
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
                //await DependencyService.Get<ISave>().SaveAndView("NhapKhoHD_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx", "application/msexcel", stream);
                //DependencyService.Get<IToast>().Show("Xuất excel thành công!");
            }
        }
        ObservableCollection<ObservableCollection<NhapKhoHDTable>> Split(ObservableCollection<NhapKhoHDTable> collection, int splitBy = 10)
        {
            var result = collection
                       .Select((x, i) => new { index = i, item = x })
                       .GroupBy(x => x.index / splitBy, x => x.item)
                       .Select(g => new ObservableCollection<NhapKhoHDTable>(g));
            return new ObservableCollection<ObservableCollection<NhapKhoHDTable>>(result);
        }
        ObservableCollection<ObservableCollection<NhapKhoHDTable>> Group(ObservableCollection<NhapKhoHDTable> collection)
        {
            var result = collection
                       .GroupBy(s => s.HopDong)
                       .Select(g => new ObservableCollection<NhapKhoHDTable>(g));
            return new ObservableCollection<ObservableCollection<NhapKhoHDTable>>(result);
        }

        public async void exportERP()
        {
            ObservableCollection<ObservableCollection<NhapKhoHDTable>> groupLon = new ObservableCollection<ObservableCollection<NhapKhoHDTable>>();
            groupLon = Group(NKHDTable);
            for (int i = 0; i < groupLon.Count; i++)
            {
                ObservableCollection<ObservableCollection<NhapKhoHDTable>> groupSplit = new ObservableCollection<ObservableCollection<NhapKhoHDTable>>();
                groupSplit = Split(groupLon[i]);
                for (int j = 0; j < groupSplit.Count; j++)
                {
                    NKHDTableERP = new ObservableCollection<NhapKhoHDTableERP>();
                    int index = 0;
                    foreach (NhapKhoHDTable ct in groupSplit[j])
                    {
                        if (index == 0)
                        {
                            NhapKhoHDTableERP ds = new NhapKhoHDTableERP();
                            ds.Column1 = @"\{ENTER}";
                            ds.Column2 = @"\{TAB 3}";
                            ds.Column3 = ct.HopDong;
                            ds.Column4 = @"\{TAB}";
                            ds.Column5 = @"\%N";
                            ds.Column6 = @"\{TAB}";
                            ds.Column7 = groupLon[i].Count.ToString();
                            ds.Column8 = @"\{TAB 2}";
                            ds.Column9 = "inventory";
                            ds.Column10 = @"\{TAB 7}";
                            ds.Column11 = DateTime.Now.ToString();
                            ds.Column12 = @"\{TAB}";
                            ds.Column13 = @"\%S";
                            ds.Column14 = @"*SL0,5";
                            ds.Column15 = @"\{TAB}";
                            ds.Column16 = @"\%I";
                            ds.Column17 = ct.MaCode;
                            ds.Column18 = "";
                            ds.Column19 = "";
                            ds.Column20 = @"\{DOWN}";
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
                            NKHDTableERP.Add(ds);
                        }
                        else if (index == groupSplit[j].Count - 1)
                        {
                            NhapKhoHDTableERP ds = new NhapKhoHDTableERP();
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
                            ds.Column17 = ct.MaCode;
                            ds.Column18 = "";
                            ds.Column19 = "";
                            ds.Column20 = @"\{TAB}";
                            ds.Column21 = @"\%D";
                            ds.Column22 = @"\^S";
                            ds.Column23 = "*SL0,5";
                            ds.Column24 = @"\%O";
                            ds.Column25 = @"\^L";
                            ds.Column26 = @"\{TAB 2}";
                            ds.Column27 = "01";
                            ds.Column28 = @"\%O";
                            ds.Column29 = @"\{TAB}";
                            ds.Column30 = @"\%O";
                            ds.Column31 = "*SL0,5";
                            ds.Column32 = "Nhập kho theo hợp đồng " + groupSplit[i][0].HopDong + ", Hóa đơn " + groupSplit[i][0].GTGT + ", BBGN " + groupSplit[i][0].BBGN;
                            ds.Column33 = @"\^S";
                            ds.Column34 = @"\%O";
                            NKHDTableERP.Add(ds);
                        }
                        else
                        {
                            NhapKhoHDTableERP ds = new NhapKhoHDTableERP();
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
                            ds.Column17 = ct.MaCode;
                            ds.Column18 = "";
                            ds.Column19 = "";
                            ds.Column20 = @"\{DOWN}";
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
                            NKHDTableERP.Add(ds);
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
                    //await DependencyService.Get<ISave>().SaveAndView("ERP_DT01P-RF.3.60.05.130.VIE.SE.000.D43.A.1_T01_" + UserName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx", "application/msexcel", stream, "ERP");
                    string filename = UserName + gettenfile(groupSplit[j][0].MaKho, "331", "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-ERP-" + (j + 1).ToString("00") + ".xlsx";
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
                    if (i == 0 && j == 0)
                    {
                        foreach (NhapKhoHDTable ct in NKHDTable)
                        {
                            dsSerial += ct.MaCode + "_";
                        }
                        dsSerial = dsSerial.Substring(0, dsSerial.Length - 1);
                        FileDaGoiTable ma = new FileDaGoiTable();
                        ma.ChucNang = 1;
                        ma.MaQRCode = dsSerial;
                        ma.TenFile = filename.Substring(0, filename.IndexOf("-ERP-", StringComparison.Ordinal));
                        ma.User = UserName;
                        ma.NgayXL = DateTime.Now;
                        idFile = dataAccess.SaveRecordFile(ma);
                        foreach (NhapKhoHDTable ct in NKHDTable)
                        {
                            ct.IDFile = idFile;
                            dataAccess.SaveRecordNhapKhoHD(ct);
                        }
                        fileNameex = ma.TenFile + ".jpg";
                    }  
                }
            }
        }

        public async void exportCMIS()
        {
            NKHDTableCMIS = new ObservableCollection<NhapKhoHDTableCMIS>();
            foreach (NhapKhoHDTable ct in NKHDTable)
            {
                NhapKhoHDTableCMIS ds = new NhapKhoHDTableCMIS();
                string[] kdinhInfo = ct.CheckedInfo.Split('|');
                ds.MA_DVI_SD = userInfo.MA_DVI_QLY;
                ds.SO_CTO = ct.MaCode;
                ds.MA_CLOAI = ct.CLoai;
                ds.NAM_SX = ct.NamSX;
                ds.LOAI_SOHUU = "0";
                ds.NGAY_NHAP = ct.NgayXL.ToString("dd/MM/yyyy");
                ds.MA_KHO = "KD";
                ds.SO_BBAN = ct.BBGN;
                ds.MA_NVIENKD = ct.Ma_NvienKD;
                ds.NGUOI_SUA = userInfo.USERID;
                ds.MA_CNANG = "1";
                ds.SO_BBAN_KD = (kdinhInfo != null) ? kdinhInfo[0] : "";
                ds.MA_DVIKD = getMA_DVIKD(ct.Ten_Dviqly);
                ds.TINH_TRANG = "1";
                ds.NGAY_KDINH = (kdinhInfo != null) ? DateTime.Parse(kdinhInfo[1]).ToString("dd/MM/yyyy") : "";
                ds.MA_CHIKD = ct.Ma_ChiKD;
                ds.SO_CHIKD = "2";
                ds.BCS = "KT;VC";
                ds.HSN = "1";
                ds.MTEM_KD = "1";
                ds.KIM_CHITAI = ds.MA_CHIKD;
                ds.SO_CHITAI = "1";
                ds.SO_HDONG = "1";
                ds.SERY_TEMKD = (kdinhInfo != null) ? kdinhInfo[3] : "";
                ds.THU_NGUYEN = "1";
                ds.TYSO_TI = "1";
                ds.TYSO_TU = "1";
                ds.MTEM_CQ = "1";
                ds.SERY_TEMCQ = "0";
                ds.MA_NVIEN = "1";
                ds.MA_NVIENLT = "";
                ds.SLAN_LT = "";
                ds.NGAY_LTRINH = "";
                ds.LOG_ERROR = "";
                ds.TTRANG_CH = "0";
                ds.HAN_KDINH = (ct.CheckedEXDate != null) ? DateTime.ParseExact(ct.CheckedEXDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                NKHDTableCMIS.Add(ds);
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
            string filename = UserName + gettenfile(NKHDTable[0].MaKho, "331", "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-CMIS-01" + ".xlsx";
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
                //Shell.Current.Navigation.PushAsync(new LichSuPage(1));          
                Shell.Current.Navigation.PushModalAsync(new LichSuPage(1));

            });
        }
    }
}