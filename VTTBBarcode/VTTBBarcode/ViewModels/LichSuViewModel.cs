using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using VTTBBarcode.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.ViewModels
{
    public class LichSuViewModel : BaseViewModel
    {
        ObservableCollection<FileDaGoiTable> _listTemp;
        ObservableCollection<FileDaGoiTable> _lichSu;
        public ObservableCollection<FileDaGoiTable> LichSu
        {
            get { return _lichSu; }
            set
            {
                _lichSu = value;
                OnPropertyChanged("LichSu");
            }
        }
        public FileDaGoiTable _selectItem;
        public FileDaGoiTable SelectItem
        {
            get => _selectItem;
            set => SetProperty(ref _selectItem, value);
        }
        int s = 0;
        public int cn = 0;
        bool _openPopupThung;
        public bool IsOpenPopupThung { get => _openPopupThung; set => SetProperty(ref _openPopupThung, value); }
        ObservableCollection<RecordFileCT> _dSThung;
        public ObservableCollection<RecordFileCT> DSThung
        {
            get { return _dSThung; }
            set
            {
                _dSThung = value;
                OnPropertyChanged("DSThung");
            }
        }
        public Command AcceptCommand { get; }
        public Command BackCommand { get; }
        //string _imgUrl;
        //public string ImgUrl { get => _imgUrl; set => SetProperty(ref _imgUrl, value); }
        //public EncodingOptions BarcodeOptions => new EncodingOptions() { Height = 500, Width = 500, PureBarcode = true };

        public LichSuViewModel(int cng)
        {
            cn = cng;
            switch (cn)
            {
                case 1:
                    Title = "Nhập kho theo hợp đồng";
                    break;
                case 2:
                    Title = "Nhập kho";
                    break;
                case 3:
                    Title = "Chuyển kho";
                    break;
                case 4:
                    Title = "Xuất kho";
                    break;
            }
            IsOpenPopupThung = false;
            AcceptCommand = new Command(OnAcceptCLicked);
            BackCommand = new Command(OnBackCLicked);
            LoadData();
        }
        private void LoadData()
        {
            _listTemp = new ObservableCollection<FileDaGoiTable>(dataAccess.LoadRecordFile(UserName, cn));
            foreach (FileDaGoiTable t in _listTemp)
            {
                s++;
                t.STT = s;
                //ImgUrl = t.MaQRCode; //test
            }
            LichSu = _listTemp;
        }
        private void OnAcceptCLicked(object obj)
        {
            IsOpenPopupThung = false;
        }

        public void ShowDataThung()
        {
            DSThung = new ObservableCollection<RecordFileCT>(dataAccess.LoadRecordFileCT(UserName, cn, SelectItem.ID));
            IsOpenPopupThung = true;
        }
        private void OnBackCLicked(object obj)
        {
            Shell.Current.Navigation.PopModalAsync();
        }
    }
}