using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VTTBBarcode.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        string _fullname;
        string _password;
        public string Password { get => _password; set { SetProperty(ref _password, value); } }
        public string FullName { get => _fullname; set { SetProperty(ref _fullname, value); } }
        bool _toggledNhoPass;
        public bool ToggledNhoPass
        {
            get { return _toggledNhoPass; }
            set
            {
                {
                    _toggledNhoPass = value;
                }
                OnPropertyChanged("ToggledNhoPass");
            }
        }
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            ToggledNhoPass = Preferences.Get(AprroveNhoPass, false);
            //FullName = Preferences.Get(UserName, "");
            //Password = Preferences.Get(Pass, ""); 
            FullName = Preferences.Get(UserNameLogin, "");
            Password = Preferences.Get(PassLogin, "");
        }

        private async void OnLoginClicked(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Password))
                {
                    DependencyService.Get<IToast>().Show(string.Format("Anh/ chị vui lòng điền đẩy đủ username và password!"));
                    return;
                }
                ShowLoading("Vui lòng đợi");
                await Task.Delay(200);
                //đổi api login mới có trả về kho
                //var responseUS = await client.GetAsync(UrlHD + String.Format("check_loggin?username_ad={0}&password_ad={1}&password={2}", FullName, Password.Replace("#", "%23").Replace("&", "%26"), PassListKho));
                //var responseU = responseUS.Content.ReadAsStringAsync().Result;

                var requestU = new RestRequest(UrlHD + "check_loggin", Method.Post);
                requestU.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                requestU.AddParameter("username_ad", FullName);
                requestU.AddParameter("password_ad", Password);
                requestU.AddParameter("password", PassListKho);
                RestResponse responseU = await clientS.ExecutePostAsync(requestU);                
                var outputU = responseU.Content.Split('>', '<'); 
                if (outputU[4] == "Flase")
                {
                    DependencyService.Get<IToast>().Show(string.Format("Thông tin đăng nhập không chính xác. Anh chị vui lòng kiểm tra lại!"));
                    HideLoading();
                    return;
                }
                userInfo = JsonConvert.DeserializeObject<ObservableCollection<UserInfo>>(outputU[4])[0];
                if (Preferences.Get(UserNameLogin, "") != FullName)
                    Preferences.Remove(UserNameLogin);
                if (Preferences.Get(PassLogin, "") != Password)
                    Preferences.Remove(PassLogin);
                UserName = FullName;
                Preferences.Set(UserNameLogin, FullName);
                if (ToggledNhoPass)
                {
                    Preferences.Set(PassLogin, Password);
                }
                else Preferences.Set(PassLogin, "");

                //var response = await client.GetAsync(UrlHD + String.Format("get_ListKho_By_Useid?userid={0}&Password={1}", UserName, PassListKho));
                //var responseContent = response.Content.ReadAsStringAsync().Result;
                //var output = responseContent.Split('>', '<');
                //listKho = JsonConvert.DeserializeObject<ObservableCollection<DanhSachKho>>(output[4]);

                var request = new RestRequest(UrlHD + "get_ListKho_By_Useid", Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("userid", UserName);
                request.AddParameter("Password", PassListKho);
                RestResponse response = await clientS.ExecutePostAsync(request);
                var output = response.Content.Split('>', '<');
                listKho = JsonConvert.DeserializeObject<ObservableCollection<DanhSachKho>>(output[4]);

                HideLoading();
            }
            catch (Exception ex)
            {
                DependencyService.Get<IToast>().Show(string.Format("Anh/ chị vui lòng kết nối với mạng nội bộ để sử dụng ứng dụng!"));
                HideLoading();
                return;
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                //if (Device.RuntimePlatform == Device.Android && DeviceInfo.Version.Major < 12)
                //    Application.Current.MainPage = new AppShell();
                //else
                //{
                //    //Ask for permission first
                //    bool allowed = false;
                //    allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
                //    if (allowed)
                //        Application.Current.MainPage = new AppShell();
                //    else
                //        DependencyService.Get<IToast>().Show("Anh/ chị vui lòng cấp quyền Camera để sử dụng ứng dụng!");
                //}
                if (Device.RuntimePlatform == Device.Android && DeviceInfo.Version.Major < 8)
                    Application.Current.MainPage = new AppShell();
                else //if (Device.RuntimePlatform == Device.Android)
                {
                    //Ask for permission first
                    bool allowed = false;
                    allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
                    if (allowed)
                        Application.Current.MainPage = new AppShell();
                    else
                        DependencyService.Get<IToast>().Show("Anh/ chị vui lòng cấp quyền Camera để sử dụng ứng dụng!");
                }
                //else
                //{
                //    //Ask for permission first
                //    bool allowed = false;
                //    allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
                //    if (allowed)
                //        Application.Current.MainPage = new AppShell();
                //    else
                //        DependencyService.Get<IToast>().Show("Anh/ chị vui lòng cấp quyền Camera để sử dụng ứng dụng!");
                //}    
            });
        }

        public void SetNhoPass()
        {
            Preferences.Set(AprroveNhoPass, ToggledNhoPass);
        }
    }
}
