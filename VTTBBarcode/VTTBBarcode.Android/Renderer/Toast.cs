using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTTBBarcode.Droid.Renderer;
using VTTBBarcode.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastAndroid))]
namespace VTTBBarcode.Droid.Renderer
{
    public class ToastAndroid : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}