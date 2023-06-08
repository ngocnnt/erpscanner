using Android.App;
using Android.Content;
using Android.Media;
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

[assembly: Dependency(typeof(SoundAndroid))]
namespace VTTBBarcode.Droid.Renderer
{
    public class SoundAndroid : ISound
    {
        private MediaPlayer _mediaPlayer;
        public bool playBeepSound()
        {
            _mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.beep);
            _mediaPlayer.Start();
            return true;
        }

    }
}