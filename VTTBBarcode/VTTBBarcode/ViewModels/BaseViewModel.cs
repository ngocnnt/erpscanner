using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VTTBBarcode.Dialog;
using VTTBBarcode.Interface;
using Xamarin.Forms;

namespace VTTBBarcode.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public bool CheckInternet()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Task.Run(() => new MessageInternetProblem().Show());
            }
            return CrossConnectivity.Current.IsConnected;
        }
        public void ShowLoading(string title)
        {
            DependencyService.Get<IProcessLoader>().Show(Title);
        }
        public void HideLoading()
        {
            DependencyService.Get<IProcessLoader>().Hide();
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
