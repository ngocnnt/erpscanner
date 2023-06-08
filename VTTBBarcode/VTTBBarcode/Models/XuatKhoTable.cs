using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Models
{
    [Table("XuatKhoTable")]
    public class XuatKhoTable
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

        public string _maCode;
        [Indexed(Name = "CompositeKey", Order = 2, Unique = true)]
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

        public string _hinhThucXK;
        public string HinhThucXK
        {
            get { return _hinhThucXK; }
            set { this._hinhThucXK = value; }
        }

        public string _khoTon;
        public string KhoTon
        {
            get { return _khoTon; }
            set { this._khoTon = value; }
        }
        public string _khoTonPhu;
        public string KhoTonPhu
        {
            get { return _khoTonPhu; }
            set { this._khoTonPhu = value; }
        }

        public string _doiTac;
        public string DoiTac
        {
            get { return _doiTac; }
            set { this._doiTac = value; }
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
        public int _iDFile;
        public int IDFile
        {
            get { return _iDFile; }
            set { this._iDFile = value; }
        }
    }
}
