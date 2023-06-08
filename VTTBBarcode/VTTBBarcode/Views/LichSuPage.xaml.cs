using System;
using System.ComponentModel;
using VTTBBarcode.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Views
{
    public partial class LichSuPage : ContentPage
    {
        LichSuViewModel viewModel;
        public LichSuPage(int cn)
        {
            InitializeComponent();
            viewModel = new LichSuViewModel(cn);
            this.BindingContext = viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void gridMaCode_SelectionChanged(object sender, Syncfusion.SfDataGrid.XForms.GridSelectionChangedEventArgs e)
        {
            viewModel.ShowDataThung();
        }
    }
}