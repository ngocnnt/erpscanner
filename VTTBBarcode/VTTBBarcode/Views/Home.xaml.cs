using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            animation.Play();
            StoragePermission.Check();
        }
    }
}