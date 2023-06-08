using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class ChuyenKhoPage : ContentPage
    {
        ChuyenKhoViewModel viewModel;
        public ChuyenKhoPage()
        {
            InitializeComponent();
            viewModel = new ChuyenKhoViewModel();
            this.BindingContext = viewModel;
            viewModel.sfDataGrid = this.gridMaCode;
            viewModel.sfDataGridERP = this.gridMaCodeERP;
            viewModel.sfDataGridCMIS = this.gridMaCodeCMIS;
        }
    }
}