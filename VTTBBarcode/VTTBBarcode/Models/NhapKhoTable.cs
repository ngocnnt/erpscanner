using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTTBBarcode.Models
{
    [Table("NhapKhoTable")]
    public class NhapKhoTable
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

        public string _khoGD;
        public string KhoGD
        {
            get { return _khoGD; }
            set { this._khoGD = value; }
        }

        public string _hinhThucNK;
        public string HinhThucNK
        {
            get { return _hinhThucNK; }
            set { this._hinhThucNK = value; }
        }

        public string _chatLgVTTB;
        public string ChatLgVTTB
        {
            get { return _chatLgVTTB; }
            set { this._chatLgVTTB = value; }
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
        public string _cLoai;
        public string CLoai
        {
            get { return _cLoai; }
            set { this._cLoai = value; }
        }
        public string _namSX;
        public string NamSX
        {
            get { return _namSX; }
            set { this._namSX = value; }
        }
        public string _checkedInfo;
        public string CheckedInfo
        {
            get { return _checkedInfo; }
            set { this._checkedInfo = value; }
        }
        public string _checkedEXDate;
        public string CheckedEXDate
        {
            get { return _checkedEXDate; }
            set { this._checkedEXDate = value; }
        }
        public bool? _checkedResult;
        public bool? CheckedResult
        {
            get { return _checkedResult; }
            set { this._checkedResult = value; }
        }
        public string _ten_Dviqly;
        public string Ten_Dviqly
        {
            get { return _ten_Dviqly; }
            set { this._ten_Dviqly = value; }
        }
        public string _ma_ChiKD;
        public string Ma_ChiKD
        {
            get { return _ma_ChiKD; }
            set { this._ma_ChiKD = value; }
        }
        public string _ma_NvienKD;
        public string Ma_NvienKD
        {
            get { return _ma_NvienKD; }
            set { this._ma_NvienKD = value; }
        }
        public int _iDFile;
        public int IDFile
        {
            get { return _iDFile; }
            set { this._iDFile = value; }
        }
    }
}
