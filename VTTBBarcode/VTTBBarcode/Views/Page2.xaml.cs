using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
//using BarcodeScanner.Mobile;

namespace VTTBBarcode.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page2 : ContentPage
    {
        public string data { get; set; }
        public string format { get; set; }
        public Page2()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

        private void FlashlightButton_Clicked(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
            } 
            else Camera.TorchOn = !Camera.TorchOn;
        }

        private void Camera_OnDetected(object sender, BarcodeScanner.Mobile.OnDetectedEventArg e)
        {
            List<BarcodeScanner.Mobile.BarcodeResult> obj = e.BarcodeResults;
            string result = string.Empty;
            string result1 = string.Empty;
            for (int i = 0; i < obj.Count; i++)
            {
                result += obj[i].DisplayValue;
                result1 += obj[i].BarcodeFormat;
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                data = result;
                format = result1;
                await App.Current.MainPage.Navigation.PopModalAsync();
            });
        }

        private void Camera_OnDetected_1(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;
            string result = string.Empty;
            string result1 = string.Empty;
            for (int i = 0; i < obj.Count; i++)
            {
                result += obj[i].DisplayValue;
                result1 += obj[i].BarcodeType;
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                data = result;
                format = result1; 
                //DependencyService.Get<Interface.IToast>().Show(format);
                await App.Current.MainPage.Navigation.PopModalAsync();
            });
        }
    }
}
