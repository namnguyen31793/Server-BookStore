using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.RequestCode
{
    public static class REQUEST_API_CODE
    {
        public const int TEST = 0;
        public const int API_LOGIN = 1;
        public const int API_BILLING_IN_ORDER = 2;
        public const int API_BILLING_OUT_ODER = 3;
        public const int API_BILLING_OUT_CONFIRM = 4;
        public const int API_REGIS = 5;
        public const int API_UPDATE_INFO = 6;
        public const int API_SET_SCORE_TOUR_TETRIS = 7;
        public const int API_GET_TOP_TOUR_TETRIS = 8;
        public const int API_GET_INFO_ACTOR_TOUR_TETRIS = 9;
        public const int API_SET_SCORE_TOUR_FLAPPY_BIRD = 10;
        public const int API_GET_TOP_TOUR_FLAPPY_BIRD = 11;
        public const int API_GET_INFO_ACTOR_TOUR_FLAPPY_BIRD = 12;
        public const int API_GET_ACTOR_INFO_BY_COOKIE = 13;
        public const int API_REFRESH_TOKEN_VTV = 14;
        public const int API_GET_ACTOR_INFO_VTV = 15;
        public const int API_GET_ACTOR_BALANCE_VTV = 16;
        public const int API_LOGIN_FB = 17;
        public const int API_LOGIN_GG = 18;

        #region LAUNCHER_LOBBY
        public const int API_GET_LIST_GAME_LOBBY_BY_TYPE = 100;
        public const int API_GET_DETAIL_GAME_LOBBY = 101;
        public const int API_GET_GAME_HOT_LOBBY = 102;
        public const int API_GET_LIST_GAME_LOBBY_BY_TAG = 103;
        public const int API_GET_LIST_FRIEND_BY_ID = 104;
        public const int API_GET_LIST_INVITE_FRIEND_BY_ID = 105;
        public const int API_INVITE_FRIEND = 106;
        public const int API_ACCEPT_FRIEND = 107;
        public const int API_REMOVE_FRIEND = 108;
        public const int API_GET_LIST_GAME_LOBBY_BY_LIST_TYPE = 109;
        public const int API_GET_GAME_TAG_LOBBY = 110;
        #endregion

        #region CONFIG
        public const int CONFIG_SERVICES = 200;
        public const int CONFIG_URL = 201;
        public const int CONFIG_LOCK_FUNCTION = 202;
        public const int CONFIG_SUPPORT = 203;
        public const int CONFIG_GET_LINK_NEWS = 204;
        public const int CONFIG_GET_LINK_LOGIN = 205;
        public const int CONFIG_GET_DONATE_CONFIG = 206;
        public const int CONFIG_GET_LINK_WEB_ID = 207;
        public const int CONFIG_GET_LINK_WEB_SHOP = 208;
        public const int CONFIG_GET_LINK_URL_GAME = 209;
        public const int CONFIG_GET_LIST_TYPE_REPORT_VIDEO = 210;
        #endregion

        #region LIVE
        public const int API_LIVE_LOGIN = 300;
        public const int API_LIVE_CREATE = 301;
        public const int API_LIVE_CHANGE_STATUS = 302;
        public const int API_LIVE_GET_VIDEO = 303;
        public const int API_LIVE_GET_VIDEO_BY_TAG = 304;
        public const int API_LIVE_GET_URL_CONTENT = 305;
        public const int API_LIVE_CLEAR_VIDEO = 306;
        public const int API_LIVE_GET_TAG_VIDEO = 307;
        public const int API_LIVE_DONATE = 308;
        public const int API_LIVE_CHECK_STATUS_STREAM = 309;
        public const int API_LIVE_SUBCRIBE_KOL = 310;
        public const int API_LIVE_CHECK_SUBCRIBE_KOL_LIVESTREAM = 311;
        public const int API_LIVE_GET_USER_LIST_SUBCRIBE = 312;
        public const int API_LIVE_GET_KOL_NUMBER_SUBCRIBE = 313;
        public const int API_LIVE_GET_VIDEO_BY_KOL = 314;
        public const int API_LIVE_GET_STREAMING_SCHEDULE = 315;
        public const int API_LIVE_GET_NOTIFY_ACTION_LIVE = 316;
        public const int API_LIVE_GET_TOP_DONATE = 317;
        public const int API_LIVE_PING_LIVE_STREAM_ONLINE = 318;
        public const int API_LIVE_REPORT_CONTENT_LIVE_STREAM = 319;
        public const int API_LIVE_COMMENT_CONTENT_VIDEO = 320;
        public const int API_LIVE_GET_COMMENT_CONTENT_VIDEO = 321;
        public const int API_LIVE_SEND_LIKE_AND_DISLIKE = 322;
        #endregion

        #region GAME_EVENT  
        public const int API_GAME_EVENT_GET_JACKPOT_INFO = 401;
        public const int API_GAME_EVENT_SPIN = 402;
        public const int API_GAME_EVENT_SHOP_BY_GIFTCODE = 403;
        public const int API_GAME_EVENT_SHOP_BY_ONG = 404;
        public const int API_GAME_EVENT_SHOP_HISTORY = 405;
        public const int API_GAME_EVENT_GET_ACCOUNT_PIECE = 406;
        public const int API_PAYMENT_SHOP_BUY_ITEM = 407;
        public const int API_PAYMENT_SHOP_GET_LIST_PRODUCT = 408;
        #endregion
    }
}
