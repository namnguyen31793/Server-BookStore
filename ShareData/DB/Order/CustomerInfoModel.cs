using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Order
{
    public class CustomerInfoModel
    {
        public long CustomerId { get; set; }
        public long AccountId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public bool Defaut { get; set; }
    }
}
