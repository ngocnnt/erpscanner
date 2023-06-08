using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Models
{
    [Table("DSMaCode")]
    public class DSMaCode
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

        public int _cNang;
        [Indexed(Name = "CompositeKey", Order = 2, Unique = true)]
        [NotNull]
        public int CNang
        {
            get { return _cNang; }
            set { this._cNang = value; }
        }

        public string _maCode;
        [Indexed(Name = "CompositeKey", Order = 3, Unique = true)]
        [NotNull]
        public string MaCode
        {
            get { return _maCode; }
            set { this._maCode = value; }
        }

        public string _maVTTB;
        [NotNull]
        public string MaVTTB
        {
            get { return _maVTTB; }
            set { this._maVTTB = value; }
        }

        public string _vttB_Status;
        [NotNull]
        public string vttB_Status
        {
            get { return _vttB_Status; }
            set { this._vttB_Status = value; }
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
