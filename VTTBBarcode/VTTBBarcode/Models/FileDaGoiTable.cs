using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Models
{
    [Table("FileDaGoiTable")]
    public class FileDaGoiTable
    {
        public int _iD;
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID
        {
            get { return _iD; }
            set { this._iD = value; }
        }

        public string _user;
        [Indexed(Name = "CompositeKey", Order = 1, Unique = true)]
        [NotNull]
        public string User
        {
            get { return _user; }
            set { this._user = value; }
        }

        public string _tenFile;
        [Indexed(Name = "CompositeKey", Order = 2, Unique = true)]
        [NotNull]
        public string TenFile
        {
            get { return _tenFile; }
            set { this._tenFile = value; }
        }

        public int _chucNang;
        [NotNull]
        public int ChucNang
        {
            get { return _chucNang; }
            set { this._chucNang = value; }
        }

        public string _maQRCode;
        public string MaQRCode
        {
            get { return _maQRCode; }
            set { this._maQRCode = value; }
        }

        public DateTime _ngayXL;
        public DateTime NgayXL
        {
            get { return _ngayXL; }
            set { this._ngayXL = value; }
        }

        public int _stt;
        public int STT
        {
            get { return _stt; }
            set { this._stt = value; }
        }
    }
}
