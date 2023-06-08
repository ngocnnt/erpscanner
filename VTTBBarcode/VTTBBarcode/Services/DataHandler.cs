using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using VTTBBarcode.Interface;
using VTTBBarcode.Models;
using Xamarin.Forms;
using static VTTBBarcode.Models.clsVaribles;

namespace VTTBBarcode.Services
{
    public class DataHandler
    {
        private SQLiteConnection database;
        private static object collisionLock = new object();
        public ObservableCollection<DSMaCode> DSMaCodes { get; set; }
        public ObservableCollection<NhapKhoHDTable> NhapKhoHDTables { get; set; }
        public ObservableCollection<NhapKhoTable> NhapKhoTables { get; set; }
        public ObservableCollection<ChuyenKhoTable> ChuyenKhoTables { get; set; }
        public ObservableCollection<XuatKhoTable> XuatKhoTables { get; set; }
        public ObservableCollection<KiemKeTable> KiemKeTables { get; set; }
        public ObservableCollection<FileDaGoiTable> FileDaGoiTables { get; set; }
        public DataHandler()
        {
            database = DependencyService.Get<IDatabaseConnection>().DbConnection();
            database.CreateTable<DSMaCode>();
            database.CreateTable<NhapKhoHDTable>();
            database.CreateTable<NhapKhoTable>();
            database.CreateTable<ChuyenKhoTable>();
            database.CreateTable<XuatKhoTable>();
            database.CreateTable<KiemKeTable>();
            database.CreateTable<FileDaGoiTable>();
            this.DSMaCodes = new ObservableCollection<DSMaCode>(database.Table<DSMaCode>());
            this.NhapKhoHDTables = new ObservableCollection<NhapKhoHDTable>(database.Table<NhapKhoHDTable>());
            this.NhapKhoTables = new ObservableCollection<NhapKhoTable>(database.Table<NhapKhoTable>());
            this.ChuyenKhoTables = new ObservableCollection<ChuyenKhoTable>(database.Table<ChuyenKhoTable>());
            this.XuatKhoTables = new ObservableCollection<XuatKhoTable>(database.Table<XuatKhoTable>());
            this.KiemKeTables = new ObservableCollection<KiemKeTable>(database.Table<KiemKeTable>());
            this.FileDaGoiTables = new ObservableCollection<FileDaGoiTable>(database.Table<FileDaGoiTable>());
        }
        public IEnumerable<DSMaCode> LoadRecordByCNangUser(int cn, string us)
        {
            lock (collisionLock)
            {
                return database.Query<DSMaCode>(string.Format("SELECT * FROM DSMaCode WHERE CNang = {0} AND User = '{1}'", cn, us)).AsEnumerable();
            }
        }

        public void SaveRecord(DSMaCode p)
        {
            IEnumerable<DSMaCode> q = database.Query<DSMaCode>(string.Format("SELECT * FROM DSMaCode WHERE CNang = {0} AND User = '{1}' AND MaCode = '{2}'", p.CNang, p.User, p.MaCode)).AsEnumerable();
            if (q.Count<DSMaCode>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
        }

        public void SaveAllRecords()
        {
            foreach (var p in this.DSMaCodes)
            {
                lock (collisionLock)
                {
                    database.Insert(p);
                }
            }
        }

        public void DeleteRecord(DSMaCode p)
        {
            var ma = p.MaCode;
            if (ma != "")
            {
                lock (collisionLock)
                {
                    database.Delete(p);
                }
            }
            this.DSMaCodes.Remove(p);
        }
        public void DeleteListRecord(int cn, string us)
        {
            IEnumerable<DSMaCode> q = database.Query<DSMaCode>(string.Format("SELECT * FROM DSMaCode WHERE CNang = {0} AND User = '{1}'", cn, us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.DSMaCodes.Remove(p);
            }
        }

        public void DeleteAllRecords()
        {
            lock (collisionLock)
            {
                database.DropTable<DSMaCode>();
                database.CreateTable<DSMaCode>();
            }
            this.DSMaCodes.Clear();
            this.DSMaCodes = new ObservableCollection<DSMaCode>(database.Table<DSMaCode>());
        }


        #region NhapKhoHD
        public void SaveRecordNhapKhoHD(NhapKhoHDTable p)
        {
            IEnumerable<NhapKhoHDTable> q = database.Query<NhapKhoHDTable>(string.Format("SELECT * FROM NhapKhoHDTable WHERE User = '{0}' AND MaCode = '{1}'", p.User, p.MaCode)).AsEnumerable();
            if (q.Count<NhapKhoHDTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
            else
            {
                lock (collisionLock)
                {

                    database.Update(p);
                }
            } 
                
        }

        public IEnumerable<NhapKhoHDTable> LoadRecordNhapKhoHD(string us, uint idFile)
        {
            lock (collisionLock)
            {
                return database.Query<NhapKhoHDTable>(string.Format("SELECT * FROM NhapKhoHDTable WHERE User = '{0}' AND IDFile = '{1}'", us, idFile)).AsEnumerable();
            }
        }

        public void DeleteNhapKhoHD(string us)
        {
            IEnumerable<NhapKhoHDTable> q = database.Query<NhapKhoHDTable>(string.Format("SELECT * FROM NhapKhoHDTable WHERE User = '{0}'", us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.NhapKhoHDTables.Remove(p);
            }
        }
        #endregion

        #region NhapKho
        public void SaveRecordNhapKho(NhapKhoTable p)
        {
            IEnumerable<NhapKhoTable> q = database.Query<NhapKhoTable>(string.Format("SELECT * FROM NhapKhoTable WHERE User = '{0}' AND MaCode = '{1}'", p.User, p.MaCode)).AsEnumerable();
            if (q.Count<NhapKhoTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
            else
            {
                lock (collisionLock)
                {

                    database.Update(p);
                }
            }
        }

        public IEnumerable<NhapKhoTable> LoadRecordNhapKho(string us, uint idFile)
        {
            lock (collisionLock)
            {
                return database.Query<NhapKhoTable>(string.Format("SELECT * FROM NhapKhoTable WHERE User = '{0}' AND IDFile = '{1}'", us, idFile)).AsEnumerable();
            }
        }

        public void DeleteNhapKho(string us)
        {
            IEnumerable<NhapKhoTable> q = database.Query<NhapKhoTable>(string.Format("SELECT * FROM NhapKhoTable WHERE User = '{0}'", us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.NhapKhoTables.Remove(p);
            }
        }
        #endregion

        #region ChuyenKho
        public void SaveRecordChuyenKho(ChuyenKhoTable p)
        {
            IEnumerable<ChuyenKhoTable> q = database.Query<ChuyenKhoTable>(string.Format("SELECT * FROM ChuyenKhoTable WHERE User = '{0}' AND MaCode = '{1}'", p.User, p.MaCode)).AsEnumerable();
            if (q.Count<ChuyenKhoTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
            else
            {
                lock (collisionLock)
                {

                    database.Update(p);
                }
            }
        }

        public IEnumerable<ChuyenKhoTable> LoadRecordChuyenKho(string us, uint idFile)
        {
            lock (collisionLock)
            {
                return database.Query<ChuyenKhoTable>(string.Format("SELECT * FROM ChuyenKhoTable WHERE User = '{0}' AND IDFile = '{1}'", us, idFile)).AsEnumerable();
            }
        }

        public void DeleteChuyenKho(string us)
        {
            IEnumerable<ChuyenKhoTable> q = database.Query<ChuyenKhoTable>(string.Format("SELECT * FROM ChuyenKhoTable WHERE User = '{0}'", us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.ChuyenKhoTables.Remove(p);
            }
        }
        #endregion

        #region XuatKho
        public void SaveRecordXuatKho(XuatKhoTable p)
        {
            IEnumerable<XuatKhoTable> q = database.Query<XuatKhoTable>(string.Format("SELECT * FROM XuatKhoTable WHERE User = '{0}' AND MaCode = '{1}'", p.User, p.MaCode)).AsEnumerable();
            if (q.Count<XuatKhoTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
            else
            {
                lock (collisionLock)
                {

                    database.Update(p);
                }
            }
        }

        public IEnumerable<XuatKhoTable> LoadRecordXuatKho(string us, uint idFile)
        {
            lock (collisionLock)
            {
                return database.Query<XuatKhoTable>(string.Format("SELECT * FROM XuatKhoTable WHERE User = '{0}' AND IDFile = '{1}'", us, idFile)).AsEnumerable();
            }
        }

        public void DeleteXuatKho(string us)
        {
            IEnumerable<XuatKhoTable> q = database.Query<XuatKhoTable>(string.Format("SELECT * FROM XuatKhoTable WHERE User = '{0}'", us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.XuatKhoTables.Remove(p);
            }
        }
        #endregion

        #region KiemKe
        public void SaveRecordKiemKe(KiemKeTable p)
        {
            IEnumerable<KiemKeTable> q = database.Query<KiemKeTable>(string.Format("SELECT * FROM KiemKeTable WHERE MaKho = '{0}' AND MaKhoPhu = '{1}' AND MaCode = '{2}'", p.MaKho, p.MaKhoPhu, p.MaCode)).AsEnumerable();
            if (q.Count<KiemKeTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p);
                }
            }
        }

        public IEnumerable<KiemKeTable> LoadRecordKiemKe(string us, string kp)
        {
            lock (collisionLock)
            {
                return database.Query<KiemKeTable>(string.Format("SELECT * FROM KiemKeTable WHERE MaKho = '{0}' AND MaKhoPhu = '{1}'", us, kp)).AsEnumerable();
            }
        }

        public void DeleteKiemKe(string us, string kp)
        {
            IEnumerable<KiemKeTable> q = database.Query<KiemKeTable>(string.Format("SELECT * FROM KiemKeTable WHERE MaKho = '{0}' AND MaKhoPhu = '{1}'", us)).AsEnumerable();
            foreach (var p in q)
            {
                var ma = p.MaCode;
                if (ma != "")
                {
                    lock (collisionLock)
                    {
                        database.Delete(p);
                    }
                }
                this.KiemKeTables.Remove(p);
            }
        }
        #endregion

        #region ListFile
        public int SaveRecordFile(FileDaGoiTable p)
        {
            IEnumerable<FileDaGoiTable> q = database.Query<FileDaGoiTable>(string.Format("SELECT * FROM FileDaGoiTable WHERE User = '{0}' AND TenFile = '{1}'", p.User, p.TenFile)).AsEnumerable();
            if (q.Count<FileDaGoiTable>() == 0)
            {
                lock (collisionLock)
                {

                    database.Insert(p); 
                    return database.ExecuteScalar<int>("SELECT last_insert_rowid()");
                }
            }
            return -1;
        }

        public IEnumerable<FileDaGoiTable> LoadRecordFile(string us, int chucnang)
        {
            lock (collisionLock)
            {
                return database.Query<FileDaGoiTable>(string.Format("SELECT * FROM FileDaGoiTable WHERE User = '{0}' AND ChucNang = '{1}' ORDER BY NgayXL DESC", us, chucnang)).AsEnumerable();
            }
        }
        public IEnumerable<RecordFileCT> LoadRecordFileCT(string us, int chucnang, int id)
        {
            string db = "";
            switch(chucnang)
            {
                case 1:
                    db = "NhapKhoHDTable";
                    break;
                case 2:
                    db = "NhapKhoTable";
                    break;
                case 3:
                    db = "ChuyenKhoTable";
                    break;
                case 4:
                    db = "XuatKhoTable";
                    break;
                default:
                    break;
            }    
            lock (collisionLock)
            {
                return database.Query<RecordFileCT>(string.Format("SELECT * FROM {0} WHERE User = '{1}' AND IDFile = '{2}'", db, us, id)).AsEnumerable();
            }
        }
        #endregion
    }
}
