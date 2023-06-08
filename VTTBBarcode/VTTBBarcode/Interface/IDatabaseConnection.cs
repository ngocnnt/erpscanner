using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Interface
{
    public interface IDatabaseConnection

    {
        SQLite.SQLiteConnection DbConnection();
    }
}
