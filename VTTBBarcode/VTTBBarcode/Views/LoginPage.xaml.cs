using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        LoginViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();
            viewModel = new LoginViewModel();
            this.BindingContext = viewModel;
        }

        private void sfSwitch_StateChanged(object sender, Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs e)
        {
            viewModel.SetNhoPass();
        }
    }
}