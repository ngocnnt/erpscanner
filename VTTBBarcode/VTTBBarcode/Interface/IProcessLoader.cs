using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VTTBBarcode.Interface
{
    public interface IProcessLoader
    {
        Task Hide();
        Task Show(string title = "Loading");
    }
}
