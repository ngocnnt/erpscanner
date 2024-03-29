﻿using SQLite;
using System;
using System.IO;
using VTTBBarcode.Interface;
using VTTBBarcode.iOS.Renderer;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection))]
namespace VTTBBarcode.iOS.Renderer
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public SQLiteConnection DbConnection()
        {
            var dbName = "MaCodeDBv4.db3";
            string personalFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(personalFolder, "..", "Library");
            var path = Path.Combine(libraryFolder, dbName);
            return new SQLiteConnection(path);
        }
    }
}