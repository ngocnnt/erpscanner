using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VTTBBarcode.Interface
{
    public interface ISave
    {
        Task SaveAndView(string fileName, string contentType, MemoryStream stream, string subpath);
    }
}
