using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class KiemKePage : ContentPage
    {
        KiemKeViewModel viewModel;
        public KiemKePage()
        {
            InitializeComponent();
            viewModel = new KiemKeViewModel();
            this.BindingContext = viewModel;
            viewModel.sfDataGrid = this.gridMaCode;
            viewModel.sfDataGridERP = this.gridMaCodeERP;
        }

        private void SfComboBox_SelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            viewModel.loadKhoPhu();
        }

        private void SfComboBox_SelectionChanged_1(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            viewModel.loadDSOnHand();
        }
    }
}