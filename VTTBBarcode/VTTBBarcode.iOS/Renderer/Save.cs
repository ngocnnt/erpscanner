using AVFoundation;
using BigTed;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using VTTBBarcode.Interface;
using VTTBBarcode.iOS.Renderer;

[assembly: Xamarin.Forms.Dependency(typeof(SaveiOS))]
namespace VTTBBarcode.iOS.Renderer
{
    public class SaveiOS : ISave
    {
        public async Task SaveAndView(string fileName, String contentType, MemoryStream stream, string subpath)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "CPC_VTTB");
                Directory.CreateDirectory(directoryname);
                var path = directoryname;
                if (subpath != "")
                {
                    path = Path.Combine(directoryname, subpath);
                    Directory.CreateDirectory(path);
                }

                string filePath = Path.Combine(path, fileName);
                try
                {
                    FileStream fileStream = File.Open(filePath, FileMode.Create);
                    stream.Position = 0;
                    stream.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
                catch (Exception e)
                {
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}