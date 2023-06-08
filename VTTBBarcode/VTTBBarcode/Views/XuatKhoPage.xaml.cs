using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class XuatKhoPage : ContentPage
    {
        XuatKhoViewModel viewModel;
        public XuatKhoPage()
        {
            InitializeComponent();
            viewModel = new XuatKhoViewModel();
            this.BindingContext = viewModel;
            viewModel.sfDataGrid = this.gridMaCode;
            viewModel.sfDataGridERP = this.gridMaCodeERP;
        }
    }
}