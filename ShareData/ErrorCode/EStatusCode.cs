using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.ErrorCode
{
    public class EStatusCode
    {
        public const int SUCCESS = 0;
        public const int HAVE_ADD_FRIEND = -1;
        public const int ID_EXITS = -2;
        public const int ID_NOT_EXITS = -3;

        public const int ACCOUNT_NOT_ENOUGH_ROLE = -49;
        public const int LOGIN_EXPIRED = -50;
        public const int ACCOUNT_NOT_EXITS = -51;
        public const int INVALID_PASSWORD = -52;
        public const int ACCOUNT_EXITS = -53;
        public const int NICKNAME_NOT_EXITS = -54;
        public const int NICKNAME_EXITS = -55;
        public const int ACCOUNT_NAME_INVAILD = -56;
        public const int PASSWORD_NEW_INVAILD = -57;
        public const int NICKNAME_INVAILD = -58;
        public const int TOKEN_INVALID = -59;
        public const int PASSNEW_SAME_OLD = -60;
        public const int TRANSACTION_SPAM = -61;
        public const int ACCOUNT_BLOCK = -62;
        public const int USER_NOT_LOGIN = -63;
        public const int KOL_BLOCK = -64;
        public const int NOT_ENOUGH_RIGHTS = -65;
        public const int NOT_ENOUGH_MONEY = -66;
        public const int OUT_OF_EVENT_TIME = -67;
        public const int USER_VALIDATE = -68;
        public const int EMAIL_INVAILD = -69;
        public const int PHONE_INVAILD = -70;
        public const int EMAIL_SEND = -71;
        public const int MAIL_NOT_EXIST = -72;
        public const int BARCODE_EXIST = -73;
        public const int ACOUNT_EXIST_BARCODE = -74;
        public const int BARCODE_NOT_EXIST = -75;
        public const int ACOUNT_NOT_EXIST_BARCODE = -76;
        public const int VOURCHER_NAME_EXIST = -77;
        public const int VOURCHER_NOT_ACCEPT = -78;

        public const int TOKEN_EXPIRES = -86;
        public const int TRANSACTION_SEND_MAIL_SPAM = -87;
        public const int MAX_CUSTOMER_INFO_ORDER = -88;
        public const int DELIVERY_SQL_ERROR = -89;
        public const int CART_SQL_ERROR = -90;
        public const int PAY_SQL_ERROR = -91;
        public const int ORDER_NOT_EXIST = -92;
        public const int ORDER_NOT_DATA = -93;
        public const int ADRESS_NOT_EXITS = -94;
        public const int REFRESH_TOKEN_EXPIRES = -95;
        public const int EMAIL_NOT_ENOUGH = -96;

        public const int DATABASE_ERROR = -99;
        public const int CONNECT_ERROR = -100;
        public const int CAPTCHA_INVALID = -101;

        public const int DATA_INVAILD = -600;
        public const int FEATURE_NOT_EXITS = -601;

        public const int SYSTEM_ERROR = -9999;
        public const int SYSTEM_MAINTAIN = -9998;
        public const int SYSTEM_EXCEPTION = -9997;
        //TOPUP-CMS
        public const int NOT_FIND_TRANSACTION = -701;

    }
}
