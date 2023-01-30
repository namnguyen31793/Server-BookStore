using ShareData.DataEnum;
using ShareData.ErrorCode;
using ShareData.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsSystem.Utils
{
    public class UltilsHelper
    {
        public static bool IsNotMyIP()
        {
            return true;
        }

        public static string formatMoney(long number, bool skip = true, string name = "", string sign = "")
        {
            skip = true;
            if (number == 0) return String.Format("{0} {1}", 0, name);
            if (skip) return String.Format("{0} {1}", Format(number), name);

            double _number = number;
            string[] sizes = { "", "K", "M", "B", "T" };

            bool negative = false;
            if (number < 0) { negative = true; number = Math.Abs(number); }

            int index = 0;
            while (_number >= 1000 && index < sizes.Length - 1) { index++; _number = _number / 1000; }
            if (index > 0) { _number *= 1000; index -= 1; }

            string result = String.Format("{0:0.##}{1} {2}", Format(Math.Abs(_number)), sizes[index], name);

            if (sign == "") return (negative ? "-" : "") + result;
            else return sign + result;
        }
        public static string Format(double number)
        {
            var res = number.ToString("#,#", CultureInfo.InvariantCulture);
            return res;
        }

        public static string GetOsByType(EOSType OsType) {
            string e_deviceOs = "Web";
            if (OsType == EOSType.SOURCE_ID_WEB)
                e_deviceOs = "Web";
            if (OsType == EOSType.SOURCE_ID_ANDROID)
                e_deviceOs = "Android";
            if (OsType == EOSType.SOURCE_ID_IOS)
                e_deviceOs = "Ios";
            if (OsType == EOSType.SOURCE_ID_PC)
                e_deviceOs = "Pc";
            return e_deviceOs;
        }
        
        public static string GetMessageByErrorCode(int errorCode)
        {
            string message = "";
            switch (errorCode)
            {
                case EStatusCode.SUCCESS:
                    message = "Thành công";
                    break;
                case EStatusCode.ACCOUNT_BLOCK:
                    message = "Tài khoản của bạn đã bị khóa!";
                    break;
                case EStatusCode.SYSTEM_ERROR:
                    message = "Đã có lỗi xảy ra!";
                    break;
                case EStatusCode.DATA_INVAILD:
                    message = "Dữ liệu không hợp lệ!";
                    break;
                case EStatusCode.USER_NOT_LOGIN:
                    message = "Bạn cần đăng nhập để sử dụng chức năng này!";
                    break;
                case EStatusCode.SYSTEM_MAINTAIN:
                    message = "Hệ thống đang bảo trì, vui lòng quay lại sau ít phút!";
                    break;
                case EStatusCode.NICKNAME_INVAILD:
                    message = "Nickname không hợp lệ";
                    break;
                case EStatusCode.LOGIN_EXPIRED:
                    message = "Phiên đăng nhập hết hạn";
                    break;
                case EStatusCode.KOL_BLOCK:
                    message = "Idol đã được mời đi uống nước!";
                    break;
                case EStatusCode.CONNECT_ERROR:
                    message = "Kết nối thất bại!";
                    break;
                case EStatusCode.NOT_ENOUGH_RIGHTS:
                    message = "Không đủ quyền";
                    break;
                case EStatusCode.HAVE_ADD_FRIEND:
                    message = "Đã kết bạn!";
                    break;
                case EStatusCode.TOKEN_EXPIRES:
                    message = "Token hết hạn!";
                    break;
                case EStatusCode.SYSTEM_EXCEPTION:
                    message = "Đã có lỗi xảy ra!";
                    break;
                case EStatusCode.NOT_ENOUGH_MONEY:
                    message = "Số dư không đủ!";
                    break;
                case EStatusCode.OUT_OF_EVENT_TIME:
                    message = "Không đúng thời gian sự kiện!";
                    break;
                case EStatusCode.ID_EXITS:
                    message = "Id đã tồn tại!";
                    break;
                case EStatusCode.ID_NOT_EXITS:
                    message = "Id không tồn tại!";
                    break;
                case EStatusCode.TRANSACTION_SPAM:
                    message = "Thao tác quá nhanh!";
                    break;
                case EStatusCode.ACCOUNT_NOT_EXITS:
                    message = "Tài khoản không tồn tại!";
                    break;
                case EStatusCode.INVALID_PASSWORD:
                    message = "Mật khẩu không chính xác!";
                    break;
                case EStatusCode.ACCOUNT_EXITS:
                    message = "Tài khoản đã tồn tại!";
                    break;
                case EStatusCode.TOKEN_INVALID:
                    message = "Tài khoản của bạn đã đăng nhập ở nơi khác!";
                    break;
                case EStatusCode.USER_VALIDATE:
                    message = "Tài khoản của bạn đã kích hoạt!";
                    break;
                case EStatusCode.EMAIL_INVAILD:
                    message = "Email không đúng định dạng!";
                    break;
                case EStatusCode.PHONE_INVAILD:
                    message = "Số điện thoại không hợp lệ!";
                    break;
            }
            return message;
        }
    }
}
