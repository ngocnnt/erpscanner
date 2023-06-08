using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VTTBBarcode.Dialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageInternetProblem : PopupPage
    {
        TaskCompletionSource<Boolean> _tsk = null;
        public MessageInternetProblem()
        {
            InitializeComponent();
        }
        private async void btnOK_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAllPopupAsync(true);
            _tsk.SetResult(true);
        }
        public async Task<bool> Show()
        {
            _tsk = new TaskCompletionSource<bool>();
            await Navigation.PushPopupAsync(this);
            return await _tsk.Task;
        }
    }
}