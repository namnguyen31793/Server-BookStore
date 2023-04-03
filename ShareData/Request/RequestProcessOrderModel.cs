using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestProcessOrderModel
    {
        public long OrderId { get; set; }
        public int AllowTest { get; set; }//1-cho xem hàng, k thử, 2-cho thử, 3-không xem hàng, 4-cho xem, k thu ship, mac dinh 3
        public string PrivateDescription { get; set; } //note cua van hanh
        public int weight { get; set; } 
        public int length { get; set; } 
        public int width { get; set; } 
        public int height { get; set; }
        public long service_id { get; set; }
        public long service_type_id { get; set; }
    }
}
