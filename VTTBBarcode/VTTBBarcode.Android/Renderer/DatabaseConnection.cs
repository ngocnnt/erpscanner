using SQLite;
using System.IO;
using VTTBBarcode.Droid.Renderer;
using VTTBBarcode.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace VTTBBarcode.Droid.Renderer
{
    class DatabaseConnection : IDatabaseConnection
    {
        public SQLiteConnection DbConnection()
        {
            var dbName = "MaCodeDBv4.db3";
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return new SQLiteConnection(path);
        }
    }
}