using RestSharp;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using VTTBBarcode.Services;

namespace VTTBBarcode.Models
{
    public class clsVaribles
    {
        public static HttpClient client;
        public static RestClient clientS;
        public static DataHandler dataAccess = new DataHandler();
        public static ObservableCollection<DanhSachKho> listKho;
        public static UserInfo userInfo;
        public static ObservableCollection<DanhSachOnHand> listOnHand;
        public static ObservableCollection<DanhSachDeparment> listDeparment;
        public static ObservableCollection<DanhSachProjects> listProjects;
        public static ObservableCollection<DanhSachVendor> listVendor;
        public static ObservableCollection<DanhSachKhoSub> listKhoSub;
        public static string domain = "10.72.0.27";
        public static string Url = "https://apicongto.cpc.vn:1212/api/"; //"http://10.72.99.8:1212/api/";
        public static string UrlThung = "http://smart.cpc.vn/APIBarcode/api/QLSXGHBarcode/";
        public static string UrlUser = "http://smart.cpc.vn/DCU_ROUTER/";
        public static string UrlHD = "https://mstt.cpc.vn/Hethong/ws_mstt.asmx/";
        public static string UserName = "";
        //public static string Pass = "Pass";
        public static string UserNameLogin = "UserNameLogin";
        public static string PassLogin = "PassLogin";
        public static string AprroveNhoPass = "AprroveNhoPass";
        public static string PassListKho = "cpcit@2022";
        public static string PassSendFile = "cpcit@2021";
        public static string PassBBGN = "BBgncpcit@2022";
        public static string TThai01 = "Trên lưới";
        public static string TThai02 = "Trong kho";
        public static string TThai03 = "Dưới lưới - Ngoài kho. Công tơ xuống lưới chưa nhập kho";
        public static string TThai04 = "Dưới lưới - Ngoài kho. Công tơ đã xuất kho chưa lên lưới";
        public static string TThai05 = "Dưới lưới - Ngoài kho. Công tơ đang xuất đi gia công sửa chữa và bảo hành.";
        public static string TThai06 = "Đã phá hủy";
        public static string TThai00 = "Chưa có lịch sử";
        public static string InfoPopup1 = "InfoPopup1";
        public static string InfoPopup2 = "InfoPopup2";
        public static string InfoPopup3 = "InfoPopup3";
        public static string InfoPopup4 = "InfoPopup4";
        public static string InfoPopup5 = "InfoPopup5";
        public static string InfoPopup6 = "InfoPopup6";
        public static string InfoPopup7 = "InfoPopup7";
        public static string InfoPopup8 = "InfoPopup8";
        public enum DialogReturn
        {
            OK = 0,
            Cancel = 1,
            Repeat = 2,
            Stop = 3
        }
        public class CongTo
        {
            public string serialNum { get; set; }
            public string descriptionName { get; set; }
            public string code { get; set; }
            public string code_ERP { get; set; }
            public string type { get; set; }
            public string code_CLoai { get; set; }
            public string nhaSX { get; set; }
            public string nuocSX { get; set; }
            public string tsktChung { get; set; }
            public string tongQuanSP { get; set; }
            public string kieuPha { get; set; }
            public string un { get; set; }
            public string dienAp { get; set; }
            public string capChinhXac { get; set; }
            public string ib { get; set; }
            public string imax { get; set; }
            public string ist { get; set; }
            public string hangSoCto { get; set; }
            public string tanSo { get; set; }
            public string csBieuKienMachAp { get; set; }
            public string csTieuThuMachAp { get; set; }
            public string csBieuKienMachDong { get; set; }
            public string kichThuoc { get; set; }
            public string capBaoVe { get; set; }
            public string ipTest { get; set; }
            public string acTest { get; set; }
            public string pulseTest { get; set; }
            public string tocDoTT { get; set; }
            public string tanSoTT { get; set; }
            public string csPhatXaMax { get; set; }
            public string doNhayThu { get; set; }
            public string doRongBangTan { get; set; }
            public string tGianLuuDL { get; set; }
            public string doCaoHD { get; set; }
            public string doAmTB { get; set; }
            public string doAmTT { get; set; }
            public string doAmTBMax { get; set; }
            public string nDoLamViec { get; set; }
            public string nDoLamViecMax { get; set; }
            public string nDoLuuKho { get; set; }
            public string tTinChiTiet { get; set; }
            public string donViSoHuu { get; set; }
            public string soCheTao { get; set; }
            public string vttB_Status { get; set; }
            public string vttB_Quality { get; set; }
            public bool? checkedStatus { get; set; }
            public string checkedInfo { get; set; }
            public DateTime? checkedEXDate { get; set; }
            public bool? checkedResult { get; set; }
            public string checkedInfoOnHD { get; set; }
            public bool? maintenanceStatus { get; set; }
            public string checkedCode { get; set; }
            public string statusCheckedCode { get; set; }
            public string lyLich { get; set; }
            public string vttB_Ngay_BDong { get; set; }
            public string namSX { get; set; }
            public string kho { get; set; }
            public string ten_Kho { get; set; }
            public string kho_Phu { get; set; }
            public string nguoi_Ktra_1 { get; set; }
            public string nguoi_Ktra_2 { get; set; }
            public string ten_Dviqly { get; set; }
            public string ma_ChiKD { get; set; }
            public string ma_NvienKD { get; set; }

        }
        public class UserInfo
        {
            public string USERID { get; set; }
            public string NAME { get; set; }
            public string MA_DVI_QLY { get; set; }
            public string TEN_DVI_QLY { get; set; }
            public string ORGANIZATION_CODE { get; set; }
            public string LEDGER_ID { get; set; }
            public string ORG_ID { get; set; }
            public string FLEX_VALUE { get; set; }
        }

        public class DanhSachKho
        {
            public string MA_DVI_QLY { get; set; }
            public string MAKHO { get; set; }
            public string TENKHO { get; set; }
            public string LOAIKHO { get; set; }
            public string THANHXULY { get; set; }
        }

        public class DanhSachHopDong
        {
            public string SO_HD { get; set; }
            public string NGAY_HD { get; set; }
            public string MADV_GIAO { get; set; }
            public string TEN_DV_GIAO { get; set; }
            public string MADV_NHAN { get; set; }
            public string TEN_DV_NHAN { get; set; }
        }

        public class DanhSachBBGN
        {
            public TT_BB? TT_BB { get; set; }
            public ObservableCollection<DS_VT>? DS_VT { get; set; }
        }
        public class TT_BB
        {
            public int ID_BB { get; set; }
            public string SO_BB { get; set; }
            public string CANCU_TB { get; set; }
            public string CANCU_NGAY { get; set; }
            public string GTGT_SO { get; set; }
            public string GTGT_NGAY { get; set; }
            public string CTGH_SO { get; set; }
            public string CTGH_NGAY { get; set; }
            public string BBTN_SO { get; set; }
            public string BBTN_NGAY { get; set; }
            public string CNBH_SO { get; set; }
            public string CNBH_NGAY { get; set; }
            public string CNCL_SO { get; set; }
            public string CNCL_NGAY { get; set; }
            public string CNNG_SO { get; set; }
            public string CNNG_NGAY { get; set; }
            public string TLKT_SO { get; set; }
            public string TLKT_NGAY { get; set; }
            public int TRANG_THAI { get; set; }
            public string Ten_trangThai { get; set; }
        }

        public class DS_VT
        {
            public int ID { get; set; }
            public int ID_BB { get; set; }
            public string SO_CT { get; set; }
            public string ORGANIZATION_CODE { get; set; }
            public int ORGANIZATION_ID { get; set; }
            public string ORGANIZATION_NAME { get; set; }
            public string RECEIPT_DATE { get; set; }
            public int ITEM_ID { get; set; }
            public string SEGMENT11 { get; set; }
            public string ITEM_DESC { get; set; }
            public string PRIMARY_UOM { get; set; }
            public int TRANSACT_QTY { get; set; }
            public string NUOC_SX { get; set; }
            public string NHA_SX { get; set; }
            public string MAHIEU_NSX { get; set; }
            public string DONGIA { get; set; }
        }


        public class LichSuCTo
        {
            public string serialNum { get; set; }
            public ObservableCollection<BDongCTo>? lichSu { get; set; }
        }

        public class BDongCTo
        {
            public DateTime ngaY_BDONG { get; set; }
            public string mA_BDONG { get; set; }
            public string noI_DUNG { get; set; }
        }

        public class DanhSachOnHand
        {
            public string ORGANIZATION_ID { get; set; }
            public string INVENTORY_ITEM_ID { get; set; }
            public string SERIAL_NUMBER { get; set; }
            public string OU_NAME { get; set; }
            public string ORGANIZATION_CODE { get; set; }
            public string ORGANIZATION_NAME { get; set; }
            public string SUBINVENTORY_CODE { get; set; }
            public string LOT_NUMBER { get; set; }
            public string SEGMENT1 { get; set; }
            public string DVT { get; set; }
            public string MACL { get; set; }
            public string MAVT_ERP { get; set; }
            public string MANSX { get; set; }
            public string DESCRIPTION { get; set; }
            public string MCLVT_DESCRIPTION { get; set; }
            public string TRANSACTION_DATE { get; set; }
            public string ON_HAND { get; set; }
        }
        public class DanhSachDeparment
        {
            public string FLEX_VALUE_MEANING { get; set; }
            public string DESCRIPTION { get; set; }
        }
        public class DanhSachProjects
        {
            public string SEGMENT1 { get; set; }
            public string DESCRIPTION { get; set; }
        }
        public class DanhSachVendor
        {
            public string SUPPLIER_NUMBER { get; set; }
            public string VENDOR_NAME { get; set; }
            public string MA_SO_THUE { get; set; }
            public string ADDRESS { get; set; }
            public string ORG_ID { get; set; }
        }
        public class DanhSachKhoSub
        {
            public string ORGANIZATION_ID { get; set; }
            public string ORGANIZATION_CODE { get; set; }
            public string ORGANIZATION_NAME { get; set; }
            public string SECONDARY_INVENTORY_NAME { get; set; }
            public string DESCRIPTION { get; set; }
        }

        public class FilePostRequest
        {
            public string File_name { get; set; }
            public byte[] f { get; set; }
            public string password { get; set; }
        }

        public class RecordFileCT
        {
            public string MaCode { get; set; }
            public string vttB_Status { get; set; }

        }

        public static string getReasonCodeC(string biendong)
        {
            switch (biendong)
            {
                case "331":
                    return "01";
                case "33":
                    return "10";
                case "45":
                    return "12";
                case "333":
                    return "18";
                case "332":
                    return "23";
                case "35":
                    return "56";
                case "46":
                    return "56";
                case "421":
                    return "46";
                case "423":
                    return "50";
                case "371":
                    return "48";
                case "36":
                    return "43";
                case "372":
                    return "63";
                default:
                    return "";
            }
        }
        public static string getReasonCodeD(string biendong)
        {
            switch (biendong)
            {
                case "331":
                    return "N001";
                case "33":
                    return "N001";
                case "45":
                    return "N025";
                case "333":
                    return "N003";
                case "332":
                    return "N003";
                case "421":
                    return "X017";
                case "423":
                    return "X015";
                case "371":
                    return "X002";
                case "36":
                    return "X027";
                case "372":
                    return "X002";
                default:
                    return "";
            }
        }

        public static string gettenfile(string makho, string biendong, string chatluong)
        {
            switch (biendong)
            {
                //'User + Mã code + Mã Kho + Mã Reason + Mã Source code + Cột B + YYYYMMDDHHMMSS +ERP/CMIS+Mã NV CMIS (nếu có)+##
                //'Hieudn.N01.HGJ.N001.DT01P-RF.3.60.05.130.VIE.SE.D50.D43.F.0.20230223101830.ERP.01
                case "331":
                    return "-T01-" + makho + "-N001-DT01P-RF-3.60.05.130.VIE.SE.000-D43-A-1-";
                case "33":
                    return "-N10-" + makho + "-N001-DT01P-RF-3.60.05.130.VIE.SE.000-D43-A-1-";
                case "45":
                    return (chatluong == "D50") ? "-N12-" + makho + "-N025-DT01P-RF-3.60.05.130.VIE.SE.D50-D43-F-0-" : "-N12-" + makho + "-N025-DT01P-RF-3.60.05.130.VIE.SE.C70-D43-E-0-";
                case "333":
                    return (chatluong == "D50") ? "-N18-" + makho + "-N003-DT01P-RF-3.60.05.130.VIE.SE.D50-D43-F-0-" : "-N18-" + makho + "-N003-DT01P-RF-3.60.05.130.VIE.SE.A70-D43-E-1-";
                case "332":
                    return (chatluong == "D50") ? "-N23-" + makho + "-N003-DT01P-RF-3.60.05.130.VIE.SE.D50-D43-H-0-" : "-N23-" + makho + "-N003-DT01P-RF-3.60.05.130.VIE.SE.A70-D43-H-1-";
                case "35":
                    return "-X56-" + makho + "-C003-DT01P-RF-3.60.05.130.VIE.SE.000-D43-P-1-";
                case "46":
                    return "-X56-" + makho + "-C004-DT01P-RF-3.60.05.130.VIE.SE.C70-D43-P-0-";
                case "421":
                    return "-X46-" + makho + "-X017-DT01P-RF-3.60.05.130.VIE.SE.000-D43-E-1-";
                case "423":
                    return "-X50-" + makho + "-X015-DT01P-RF-3.60.05.130.VIE.SE.000-D43-E-1-";
                case "422":
                    return "-X47-" + makho + "-X028-DT01P-RF-3.60.05.130.VIE.SE.000-D43-E-1-";
                case "371":
                    return "-X48-" + makho + "-X002-DT01P-RF-3.60.05.130.VIE.SE.C70-D43-E-0-";
                case "36":
                    return "-X43-" + makho + "-X027-DT01P-RF-3.60.05.130.VIE.SE.D50-D43-E-0-";
                case "372":
                    return "-X63-" + makho + "-X002-DT01P-RF-3.60.05.130.VIE.SE.C70-D43-H-1-";
                default:
                    return "";
            }
        }

        public static string getMA_DVIKD(string ten_DVIKD)
        {
            switch (ten_DVIKD)
            {
                case "Công ty TNHH MTV Thí nghiệm điện miền Trung":
                    return "ETC";
                case "Trung tâm Thí nghiệm điện Quảng Bình":
                    return "ETC01";
                case "Trung tâm Thí nghiệm điện Quảng Trị":
                    return "ETC02";
                case "Trung tâm Thí nghiệm điện Thừa Thiên Huế":
                    return "ETC03";
                case "Trung tâm Thí nghiệm điện Đà Nẵng":
                    return "ETC04";
                case "Trung tâm Thí nghiệm điện Quảng Nam":
                    return "ETC05";
                case "Trung tâm Thí nghiệm điện Quảng Ngãi":
                    return "ETC06";
                case "Trung tâm Thí nghiệm điện Bình Định":
                    return "ETC07";
                case "Trung tâm Thí nghiệm điện Phú Yên":
                    return "ETC08";
                case "Trung tâm Thí nghiệm điện Khánh Hòa":
                    return "ETC09";
                case "Trung tâm Thí nghiệm điện Gia Lai":
                    return "ETC10";
                case "Trung tâm Thí nghiệm điện Kon Tum":
                    return "ETC11";
                case "Trung tâm Thí nghiệm điện Đăk Lăk":
                    return "ETC12";
                case "Trung tâm Thí nghiệm điện Đăk Nông":
                    return "ETC13";
                case "Trung tâm sản xuất thiết bị đo điện tử Điện lực miền Trung":
                    return "EMEC";
                default:
                    return "";
            }
        }
    }
}