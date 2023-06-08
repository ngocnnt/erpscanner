using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Models
{
    [Table("KiemKeTable")]
    public class KiemKeTable
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

        public string _maKho;
        public string MaKho
        {
            get { return _maKho; }
            set { this._maKho = value; }
        }
        public string _maKhoPhu;
        public string MaKhoPhu
        {
            get { return _maKhoPhu; }
            set { this._maKhoPhu = value; }
        }

        public string _vttB_Status;
        [NotNull]
        public string vttB_Status
        {
            get { return _vttB_Status; }
            set { this._vttB_Status = value; }
        }

        public DateTime _ngayKK;
        public DateTime NgayKK
        {
            get { return _ngayKK; }
            set { this._ngayKK = value; }
        }

        public int _stt;
        public int STT
        {
            get { return _stt; }
            set { this._stt = value; }
        }

        public string _checkedEXDate;
        public string CheckedEXDate
        {
            get { return _checkedEXDate; }
            set { this._checkedEXDate = value; }
        }
        public string _checkedResult;
        public string CheckedResult
        {
            get { return _checkedResult; }
            set { this._checkedResult = value; }
        }
        public string _dVT;
        public string DVT
        {
            get { return _dVT; }
            set { this._dVT = value; }
        }
        public string _sL;
        public string SL
        {
            get { return _sL; }
            set { this._sL = value; }
        }
        public string _ketQua;
        public string KetQua
        {
            get { return _ketQua; }
            set { this._ketQua = value; }
        }
    }
}
