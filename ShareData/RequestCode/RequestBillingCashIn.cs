using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.RequestCode
{
    public class RequestBillingCashIn
    {
        public int BillingType { get; set; }
        public long Money { get; set; }
        public int NSPType { get; set; }
        public string mSeriNumber { get; set; }
        public string mCardNumber { get; set; }
        public string OrderInfo { get; set; }
    }

    public class RequestBillingCashOut
    {
        public int BillingType { get; set; }
        public long Money { get; set; }
        public int NSPType { get; set; }
        public string OrderInfo { get; set; }
    }
}
