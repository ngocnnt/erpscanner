using RestSharp;
using System;
using System.Globalization;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using VTTBBarcode.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode
{
    public partial class App : Application
    {

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA4MTIyQDMxMzgyZTMyMmUzME4rVWJvRGdVY0ZibWlYbUFBN1dyNVFjemJ5djZ5dWQzZzdMaDNEQ1hBN3M9");
            InitializeComponent();
            DependencyService.Register<IProcessLoader>();
            clsVaribles.client = new System.Net.Http.HttpClient();
            clsVaribles.clientS = new RestClient();
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
