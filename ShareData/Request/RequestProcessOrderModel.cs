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
        public long ShipMoney { get; set; } //free ship = 0
    }
}
