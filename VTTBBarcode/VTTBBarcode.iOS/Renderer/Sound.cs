using AVFoundation;
using BigTed;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;
using VTTBBarcode.Interface;
using VTTBBarcode.iOS.Renderer;

[assembly: Xamarin.Forms.Dependency(typeof(SoundiOS))]
namespace VTTBBarcode.iOS.Renderer
{
    public class SoundiOS : ISound
    {
        AVAudioPlayer _player;
        public bool playBeepSound()
        {

            var fileName = "beep.mp3";
            string sFilePath = NSBundle.MainBundle.PathForResource
              (Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
            var url = NSUrl.FromString(sFilePath);
            _player = AVAudioPlayer.FromUrl(url);
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
            };
            _player.Play();
            return true;
        }
    }
}