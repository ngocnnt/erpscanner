using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class NhapKhoHDPage : ContentPage
    {
        NhapKhoHDViewModel viewModel;
        public NhapKhoHDPage()
        {
            InitializeComponent();
            viewModel = new NhapKhoHDViewModel();
            this.BindingContext = viewModel;
            viewModel.sfDataGrid = this.gridMaCode;
            viewModel.sfDataGridERP = this.gridMaCodeERP;
            viewModel.sfDataGridCMIS = this.gridMaCodeCMIS;
        }

        private void SfComboBox_SelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            viewModel.LoadDSBBGN();
        }

        private void SfComboBox_SelectionChanged_1(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            viewModel.SelectBBGN();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}