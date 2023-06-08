using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class NhapKhoPage : ContentPage
    {
        NhapKhoViewModel viewModel;
        public NhapKhoPage()
        {
            InitializeComponent();
            viewModel = new NhapKhoViewModel();
            this.BindingContext = viewModel;
            viewModel.sfDataGrid = this.gridMaCode;
            viewModel.sfDataGridERP = this.gridMaCodeERP;
            viewModel.sfDataGridCMIS = this.gridMaCodeCMIS;
        }
    }
}