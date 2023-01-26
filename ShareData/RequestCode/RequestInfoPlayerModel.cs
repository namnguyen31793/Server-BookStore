using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.RequestCode
{
    public class RequestInfoPlayerModel
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string TrueClientIp { get; set; }
        public int OsType { get; set; }
        public string DeviceId { get; set; }
        public int MerchantId { get; set; }

    }
}
