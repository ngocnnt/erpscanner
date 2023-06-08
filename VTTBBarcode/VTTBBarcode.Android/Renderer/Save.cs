using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Environment = System.Environment;
using VTTBBarcode.Droid.Renderer;
using VTTBBarcode.Interface;
using Android.OS;

[assembly: Dependency(typeof(SaveAndroid))]
namespace VTTBBarcode.Droid.Renderer
{
    public class SaveAndroid : ISave
    {
        public async Task SaveAndView(string fileName, String contentType, MemoryStream stream, string subpath)
        {
            try
            {
                string root = null;
                //Get the root path in android device.
                /*
                    if (Android.OS.Environment.IsExternalStorageEmulated)
                    {
                        root = Android.OS.Environment.ExternalStorageDirectory.ToString();
                    }
                    else
                        root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                */
                root = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).ToString();
                if (root == "")
                    root = Android.OS.Environment.ExternalStorageDirectory.ToString();
                //Create directory and file 
                string path = "";
                Java.IO.File myDir = new Java.IO.File(root + "/CPC_VTTB");
                myDir.Mkdir();
                if (subpath != "")
                {
                    path = root + "/CPC_VTTB/" + subpath;
                    myDir = new Java.IO.File(path);
                }
                myDir.Mkdir();

                Java.IO.File file = new Java.IO.File(myDir, fileName);

                //Remove if the file exists
                if (file.Exists()) file.Delete();

                //Write the stream into the file
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(stream.ToArray());

                outs.Flush();
                outs.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }
}