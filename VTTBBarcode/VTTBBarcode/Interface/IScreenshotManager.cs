using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VTTBBarcode.Interface
{
    public interface IScreenshotManager
    {
        Task<byte[]> CaptureAsync();
    }
}
