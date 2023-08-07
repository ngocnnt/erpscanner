using Newtonsoft.Json;
using RestSharp;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VTTBBarcode.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.Dialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageXacThuc : PopupPage, INotifyPropertyChanged
    {
        TaskCompletionSource<DialogReturn> _tsk = null;
        public Command CancelCommand { get; }
        public Command OkCommand { get; }
        string _hintNguoiNhan;
        public string HintNguoiNhan { get => _hintNguoiNhan; set { _hintNguoiNhan = value; OnPropertyChanged("HintNguoiNhan"); } }
        string _nguoiNhan;
        public string NguoiNhan { get => _nguoiNhan; set { _nguoiNhan = value; OnPropertyChanged("NguoiNhan"); } }
        string _hintDienGiai;
        public string HintDienGiai { get => _hintDienGiai; set { _hintDienGiai = value; OnPropertyChanged("HintDienGiai"); } }
        string _dienGiai;
        public string DienGiai { get => _dienGiai; set { _dienGiai = value; OnPropertyChanged("DienGiai"); } }
        string _hintDViNhan;
        public string HintDViNhan { get => _hintDViNhan; set { _hintDViNhan = value; OnPropertyChanged("HintDViNhan"); } }
        ObservableCollection<DanhSachDeparment> _listDViNhan;
        public ObservableCollection<DanhSachDeparment> ListDViNhan { get => _listDViNhan; set { _listDViNhan = value; OnPropertyChanged("ListDViNhan"); } }
        DanhSachDeparment _selectedDViNhan;
        public DanhSachDeparment SelectedDViNhan { get => _selectedDViNhan; set { _selectedDViNhan = value; OnPropertyChanged("SelectedDViNhan"); } }
        ObservableCollection<DanhSachKhoSub> _listKhoPhu;
        public ObservableCollection<DanhSachKhoSub> ListKhoPhu { get => _listKhoPhu; set { _listKhoPhu = value; OnPropertyChanged("ListKhoPhu"); } }
        DanhSachKhoSub _selectedKhoPhu;
        public DanhSachKhoSub SelectedKhoPhu { get => _selectedKhoPhu; set { _selectedKhoPhu = value; OnPropertyChanged("SelectedKhoPhu"); } }
        ObservableCollection<DanhSachProjects> _listCongTrinh;
        public ObservableCollection<DanhSachProjects> ListCongTrinh { get => _listCongTrinh; set { _listCongTrinh = value; OnPropertyChanged("ListCongTrinh"); } }
        DanhSachProjects _selectedCongTrinh;
        public DanhSachProjects SelectedCongTrinh { get => _selectedCongTrinh; set { _selectedCongTrinh = value; OnPropertyChanged("SelectedCongTrinh"); } }
        ObservableCollection<DanhSachVendor> _listDVTC;
        public ObservableCollection<DanhSachVendor> ListDVTC { get => _listDVTC; set { _listDVTC = value; OnPropertyChanged("ListDVTC"); } }
        DanhSachVendor _selectedDVTC;
        public DanhSachVendor SelectedDVTC { get => _selectedDVTC; set { _selectedDVTC = value; OnPropertyChanged("SelectedDVTC"); } }
        string _hintNhap1;
        public string HintNhap1 { get => _hintNhap1; set { _hintNhap1 = value; OnPropertyChanged("HintNhap1"); } }
        bool _isVisibleNhap1;
        public bool IsVisibleNhap1 { get => _isVisibleNhap1; set { _isVisibleNhap1 = value; OnPropertyChanged("IsVisibleNhap1"); } }
        string _textNhap1;
        public string TextNhap1 { get => _textNhap1; set { _textNhap1 = value; OnPropertyChanged("TextNhap1"); } }
        string _formatTextNhap1;
        public string FormatTextNhap1 { get => _formatTextNhap1; set { _formatTextNhap1 = value; OnPropertyChanged("FormatTextNhap1"); } }
        string _hintNhap2;
        public string HintNhap2 { get => _hintNhap2; set { _hintNhap2 = value; OnPropertyChanged("HintNhap2"); } }
        bool _isVisibleNhap2;
        public bool IsVisibleNhap2 { get => _isVisibleNhap2; set { _isVisibleNhap2 = value; OnPropertyChanged("IsVisibleNhap2"); } }
        string _textNhap2;
        public string TextNhap2 { get => _textNhap2; set { _textNhap2 = value; OnPropertyChanged("TextNhap2"); } }
        bool _isVisibleChonDV;
        public bool IsVisibleChonDV { get => _isVisibleChonDV; set { _isVisibleChonDV = value; OnPropertyChanged("IsVisibleChonDV"); } }
        string _hintKhoPhu;
        public string HintKhoPhu { get => _hintKhoPhu; set { _hintKhoPhu = value; OnPropertyChanged("HintKhoPhu"); } }
        bool _isVisibleKhoPhu;
        public bool IsVisibleKhoPhu { get => _isVisibleKhoPhu; set { _isVisibleKhoPhu = value; OnPropertyChanged("IsVisibleKhoPhu"); } }
        //bool _isVisibleDVTC;
        //public bool IsVisibleDVTC { get => _isVisibleDVTC; set { _isVisibleDVTC = value; OnPropertyChanged("IsVisibleDVTC"); } }

        private string formXN = "";
        private string formMaKho = "";
        public MessageXacThuc(string form, string makho)
        {
            InitializeComponent();
            formXN = form;
            formMaKho = makho;
             OkCommand = new Command(OnXacThucClicked, Validate);
            this.PropertyChanged +=
                                  (_, __) => OkCommand.ChangeCanExecute();
            CancelCommand = new Command(OnCloseClicked);
            FormatTextNhap1 = "";
            if (form == "NK")
            {
                loadData(true, true, true, true);
                HintNguoiNhan = "Người giao hàng";
                HintDViNhan = "Đơn vị nhập";
                HintDienGiai = "Diễn giải";
                HintNhap1 = "Thành tiền";
                HintKhoPhu = "Kho phụ";
                IsVisibleNhap1 = true;
                IsVisibleNhap2 = false;
                IsVisibleChonDV = true;
                IsVisibleKhoPhu = true;
                FormatTextNhap1 = "n0";
                NguoiNhan = Preferences.Get(InfoPopup1NKSave, "");
                DienGiai = Preferences.Get(InfoPopup3NKSave, "");
                TextNhap1 = Preferences.Get(InfoPopup7NKSave, "");
            }
            if (form == "CK")
            {
                loadData(false, true, true, true);
                HintNguoiNhan = "Người vận chuyển";
                HintDienGiai = "Căn cứ";
                HintKhoPhu = "Kho đến phụ";
                IsVisibleNhap1 = false;
                IsVisibleNhap2 = false;
                IsVisibleChonDV = false;
                IsVisibleKhoPhu = true;
                NguoiNhan = Preferences.Get(InfoPopup1CKSave, "");
                DienGiai = Preferences.Get(InfoPopup3CKSave, "");
            }
            if (form == "XK")
            {
                loadData(true, true, true, false);
                HintNguoiNhan = "Người nhận hàng";
                HintDViNhan = "Địa chỉ (Bộ phận) nhận";
                HintDienGiai = "Diễn giải";
                IsVisibleNhap1 = true;
                IsVisibleNhap2 = true;
                HintNhap1 = "Số Tài khoản";
                HintNhap2 = "Mã Tài khoản";
                IsVisibleChonDV = true;
                IsVisibleKhoPhu = false;
                FormatTextNhap1 = "";
                NguoiNhan = Preferences.Get(InfoPopup1XKSave, "");
                DienGiai = Preferences.Get(InfoPopup3XKSave, "");
                TextNhap1 = Preferences.Get(InfoPopup7XKSave, "");
                TextNhap2 = Preferences.Get(InfoPopup8XKSave, "");
            }
            BindingContext = this;
            numericTextBox.Culture = new System.Globalization.CultureInfo("en-US");
        }

        private async void OnCloseClicked(object obj)
        {
            await Navigation.PopPopupAsync();
            _tsk.SetResult(DialogReturn.Cancel);
        }

        private async void OnXacThucClicked()
        {
            try
            {
                if (formXN == "NK")
                {
                    InfoPopup1NK = NguoiNhan;
                    InfoPopup2NK = (SelectedDViNhan != null) ? SelectedDViNhan.FLEX_VALUE_MEANING : "";
                    InfoPopup3NK = DienGiai;
                    InfoPopup4NK = (SelectedKhoPhu != null) ? SelectedKhoPhu.SECONDARY_INVENTORY_NAME : "";
                    InfoPopup5NK = (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.SEGMENT1 == "/") ? "" : SelectedCongTrinh.SEGMENT1);
                    InfoPopup6NK = (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_ID == "/") ? "" : SelectedDVTC.VENDOR_ID);
                    if (TextNhap1 != null)
                        InfoPopup7NK = string.Format("{0:#,##0.##}", decimal.Parse(TextNhap1));

                    Preferences.Set(InfoPopup1NKSave, NguoiNhan);
                    Preferences.Set(InfoPopup2NKSave, (SelectedDViNhan != null) ? JsonConvert.SerializeObject(SelectedDViNhan) : "");
                    Preferences.Set(InfoPopup3NKSave, DienGiai);
                    Preferences.Set(InfoPopup4NKSave, (SelectedKhoPhu != null) ? JsonConvert.SerializeObject(SelectedKhoPhu) : "");
                    Preferences.Set(InfoPopup5NKSave, (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.DESCRIPTION == "/") ? "" : JsonConvert.SerializeObject(SelectedCongTrinh)));
                    Preferences.Set(InfoPopup6NKSave, (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_NAME == "/") ? "" : JsonConvert.SerializeObject(SelectedDVTC)));
                    Preferences.Set(InfoPopup7NKSave, TextNhap1);
                }

                if (formXN == "CK")
                {
                    InfoPopup1CK = NguoiNhan;
                    InfoPopup3CK = DienGiai;
                    InfoPopup4CK = (SelectedKhoPhu != null) ? SelectedKhoPhu.SECONDARY_INVENTORY_NAME : "";
                    InfoPopup5CK = (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.SEGMENT1 == "/") ? "" : SelectedCongTrinh.SEGMENT1);
                    InfoPopup6CK = (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_ID == "/") ? "" : SelectedDVTC.VENDOR_ID);

                    Preferences.Set(InfoPopup1CKSave, NguoiNhan);
                    Preferences.Set(InfoPopup3CKSave, DienGiai);
                    Preferences.Set(InfoPopup4CKSave, (SelectedKhoPhu != null) ? JsonConvert.SerializeObject(SelectedKhoPhu) : "");
                    Preferences.Set(InfoPopup5CKSave, (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.DESCRIPTION == "/") ? "" : JsonConvert.SerializeObject(SelectedCongTrinh)));
                    Preferences.Set(InfoPopup6CKSave, (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_NAME == "/") ? "" : JsonConvert.SerializeObject(SelectedDVTC)));
                }

                if (formXN == "XK")
                {
                    InfoPopup1XK = NguoiNhan;
                    InfoPopup2XK = (SelectedDViNhan != null) ? SelectedDViNhan.FLEX_VALUE_MEANING : "";
                    InfoPopup3XK = DienGiai;
                    InfoPopup5XK = (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.SEGMENT1 == "/") ? "" : SelectedCongTrinh.SEGMENT1);
                    InfoPopup6XK = (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_ID == "/") ? "" : SelectedDVTC.VENDOR_ID);
                    if (TextNhap1 != null)
                        InfoPopup7XK = string.Format("{0:#,##0.##}", decimal.Parse(TextNhap1));
                    InfoPopup8XK = TextNhap2;
                    Preferences.Set(InfoPopup1XKSave, NguoiNhan);
                    Preferences.Set(InfoPopup2XKSave, (SelectedDViNhan != null) ? JsonConvert.SerializeObject(SelectedDViNhan) : "");
                    Preferences.Set(InfoPopup3XKSave, DienGiai);
                    Preferences.Set(InfoPopup5XKSave, (SelectedCongTrinh == null) ? "" : ((SelectedCongTrinh.DESCRIPTION == "/") ? "" : JsonConvert.SerializeObject(SelectedCongTrinh)));
                    Preferences.Set(InfoPopup6XKSave, (SelectedDVTC == null) ? "" : ((SelectedDVTC.VENDOR_NAME == "/") ? "" : JsonConvert.SerializeObject(SelectedDVTC)));
                    Preferences.Set(InfoPopup7XKSave, TextNhap1);
                    Preferences.Set(InfoPopup8XKSave, TextNhap2);
                }

                await Navigation.PopPopupAsync();
                _tsk.SetResult(DialogReturn.OK);
            }
            catch (Exception ex)
            {

            }

        }
        private bool Validate()
        {
            //return true;
            try
            {
                //if (String.IsNullOrWhiteSpace(NguoiNhan) || String.IsNullOrWhiteSpace(SelectedDViNhan.DESCRIPTION) || String.IsNullOrWhiteSpace(DienGiai) || String.IsNullOrWhiteSpace(SelectedKhoPhu.ORGANIZATION_CODE) || String.IsNullOrWhiteSpace(SelectedCongTrinh.SEGMENT1) || String.IsNullOrWhiteSpace(SelectedDVTC.SUPPLIER_NUMBER) || ((formXN != "CK") ? String.IsNullOrWhiteSpace(TextNhap1) : false) || ((formXN == "XK") ? String.IsNullOrWhiteSpace(TextNhap2) : false))

                //if (String.IsNullOrWhiteSpace(NguoiNhan) || String.IsNullOrWhiteSpace(SelectedDViNhan.DESCRIPTION) || String.IsNullOrWhiteSpace(DienGiai) || String.IsNullOrWhiteSpace(SelectedKhoPhu.ORGANIZATION_CODE) || ((formXN != "CK") ? String.IsNullOrWhiteSpace(TextNhap1) : false) || ((formXN == "XK") ? String.IsNullOrWhiteSpace(TextNhap2) : false))
                //    return false;
                //else return true;

                switch(formXN)
                {
                    case "NK":
                        if (String.IsNullOrWhiteSpace(NguoiNhan) || String.IsNullOrWhiteSpace(SelectedDViNhan.DESCRIPTION) || String.IsNullOrWhiteSpace(DienGiai) || String.IsNullOrWhiteSpace(SelectedKhoPhu.ORGANIZATION_CODE) || String.IsNullOrWhiteSpace(TextNhap1))
                            return false;
                        else return true;
                    case "CK":
                        if (String.IsNullOrWhiteSpace(NguoiNhan) || String.IsNullOrWhiteSpace(DienGiai) || String.IsNullOrWhiteSpace(SelectedKhoPhu.ORGANIZATION_CODE))
                            return false;
                        else return true;
                    case "XK":
                        if (String.IsNullOrWhiteSpace(NguoiNhan) || String.IsNullOrWhiteSpace(SelectedDViNhan.DESCRIPTION) || String.IsNullOrWhiteSpace(DienGiai) || String.IsNullOrWhiteSpace(TextNhap1) || String.IsNullOrWhiteSpace(TextNhap2))
                            return false;
                        else return true;
                    default:
                        return false;
                }    
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DialogReturn> Show()
        {
            _tsk = new TaskCompletionSource<DialogReturn>();
            await Navigation.PushPopupAsync(this);
            return await _tsk.Task;
        }

        private async void loadData(bool Deparment, bool Projects, bool Vendor, bool Suborg)
        {
            try
            {
                DependencyService.Get<IProcessLoader>().Show("Vui lòng đợi");
                await Task.Delay(200);
                if (Deparment)
                {
                    //var response = await client.GetAsync(UrlHD + String.Format("get_Deparment_receipt?p_flexvalue={0}&Password={1}", userInfo.FLEX_VALUE, PassListKho));
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    //var output = responseContent.Split('>', '<');

                    var request = new RestRequest(UrlHD + "get_Deparment_receipt", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("p_flexvalue", userInfo.FLEX_VALUE);
                    request.AddParameter("Password", PassListKho);
                    RestResponse response = await clientS.ExecutePostAsync(request);
                    var output = response.Content.Split('>', '<');
                    listDeparment = JsonConvert.DeserializeObject<ObservableCollection<DanhSachDeparment>>(output[4]);
                    ListDViNhan = listDeparment;
                    string json = "";
                    if (formXN == "NK")
                        json = Preferences.Get(InfoPopup2NKSave, "");
                    else
                        json = Preferences.Get(InfoPopup2XKSave, "");
                    if (json != "")
                    {
                        DanhSachDeparment temp = JsonConvert.DeserializeObject<DanhSachDeparment>(json);
                        if (ListDViNhan.Any(x => x.DESCRIPTION == temp.DESCRIPTION && x.FLEX_VALUE_MEANING == temp.FLEX_VALUE_MEANING))
                            SelectedDViNhan = temp;
                    }
                }
                if (Projects)
                {
                    //var response = await client.GetAsync(UrlHD + String.Format("get_Projects_all?org_id={0}&Password={1}", userInfo.ORG_ID, PassListKho));
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    //var output = responseContent.Split('>', '<');
                    var request = new RestRequest(UrlHD + "get_Projects_all", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("org_id", userInfo.ORG_ID);
                    request.AddParameter("Password", PassListKho);
                    RestResponse response = await clientS.ExecutePostAsync(request);
                    var output = response.Content.Split('>', '<');
                    listProjects = JsonConvert.DeserializeObject<ObservableCollection<DanhSachProjects>>(output[4]);
                    DanhSachProjects item = new DanhSachProjects();
                    item.SEGMENT1 = "/";
                    item.DESCRIPTION = "/";
                    listProjects.Insert(0, item);
                    ListCongTrinh = listProjects;
                    string json = "";
                    if (formXN == "NK")
                        json = Preferences.Get(InfoPopup5NKSave, "");
                    else if (formXN == "CK")
                        json = Preferences.Get(InfoPopup5CKSave, "");
                    else
                        json = Preferences.Get(InfoPopup5XKSave, "");
                    if (json != "")
                    {
                        DanhSachProjects temp = JsonConvert.DeserializeObject<DanhSachProjects>(json);
                        if (ListCongTrinh.Any(x => x.DESCRIPTION == temp.DESCRIPTION && x.SEGMENT1 == temp.SEGMENT1))
                            SelectedCongTrinh = temp;
                    }
                }
                if (Vendor)
                {
                    //var response = await client.GetAsync(UrlHD + String.Format("get_Vendor_all?org_id={0}&Password={1}", userInfo.ORG_ID, PassListKho));
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    //var output = responseContent.Split('>', '<');
                    var request = new RestRequest(UrlHD + "get_Vendor_all", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("org_id", userInfo.ORG_ID);
                    request.AddParameter("Password", PassListKho);
                    RestResponse response = await clientS.ExecutePostAsync(request);
                    var output = response.Content.Split('>', '<');
                    listVendor = JsonConvert.DeserializeObject<ObservableCollection<DanhSachVendor>>(output[4]);
                    DanhSachVendor item1 = new DanhSachVendor();
                    item1.SUPPLIER_NUMBER = "/";
                    item1.VENDOR_NAME = "/";
                    item1.MA_SO_THUE = "/";
                    item1.ADDRESS = "/";
                    item1.ORG_ID = "/";
                    item1.VENDOR_ID = "/";
                    listVendor.Insert(0, item1);
                    ListDVTC = listVendor;
                    string json = "";
                    if (formXN == "NK")
                        json = Preferences.Get(InfoPopup6NKSave, "");
                    else if (formXN == "CK")
                        json = Preferences.Get(InfoPopup6CKSave, "");
                    else
                        json = Preferences.Get(InfoPopup6XKSave, "");
                    if (json != "")
                    {
                        DanhSachVendor temp = JsonConvert.DeserializeObject<DanhSachVendor>(json);
                        if (ListDVTC.Any(x => x.VENDOR_ID == temp.VENDOR_ID && x.VENDOR_NAME == temp.VENDOR_NAME))
                            SelectedDVTC = temp;
                    }
                }
                if (Suborg)
                {
                    //var response = await client.GetAsync(UrlHD + String.Format("get_Org_Suborg?org_id={0}&Password={1}", userInfo.ORG_ID, PassListKho));
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    //var output = responseContent.Split('>', '<');
                    var request = new RestRequest(UrlHD + "get_Org_Suborg", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("org_id", userInfo.ORG_ID);
                    request.AddParameter("Password", PassListKho);
                    RestResponse response = await clientS.ExecutePostAsync(request);
                    var output = response.Content.Split('>', '<');
                    ObservableCollection<DanhSachKhoSub> temp = JsonConvert.DeserializeObject<ObservableCollection<DanhSachKhoSub>>(output[4]);
                    //listKhoSub = new ObservableCollection<DanhSachKhoSub>(temp.GroupBy(o => new { o.ORGANIZATION_CODE, o.ORGANIZATION_ID, o.ORGANIZATION_NAME }).Select(o => o.FirstOrDefault()));
                    listKhoSub = new ObservableCollection<DanhSachKhoSub>(temp.Where(o => o.ORGANIZATION_CODE == formMaKho));
                    ListKhoPhu = listKhoSub;
                    if (ListKhoPhu.Count == 1)
                        SelectedKhoPhu = ListKhoPhu[0];
                    else
                    {
                        string json = "";
                        if (formXN == "NK")
                            json = Preferences.Get(InfoPopup4NKSave, "");
                        else
                            json = Preferences.Get(InfoPopup4CKSave, "");
                        if (json != "")
                        {
                            DanhSachKhoSub tempp = JsonConvert.DeserializeObject<DanhSachKhoSub>(json);
                            if (ListKhoPhu.Any(x => x.ORGANIZATION_CODE == tempp.ORGANIZATION_CODE && x.ORGANIZATION_ID == tempp.ORGANIZATION_ID && x.ORGANIZATION_NAME == tempp.ORGANIZATION_NAME && x.SECONDARY_INVENTORY_NAME == tempp.SECONDARY_INVENTORY_NAME))
                                SelectedKhoPhu = tempp;
                        }
                    }
                }
                DependencyService.Get<IProcessLoader>().Hide();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IProcessLoader>().Hide();
            }
        }
    }
}