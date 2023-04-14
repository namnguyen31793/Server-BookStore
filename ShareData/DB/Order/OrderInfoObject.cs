using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Order
{
    public class OrderInfoObject
    {
        public long OrderId { get; set; }
        public string CartId { get; set; }
        public long CustomerId { get; set; }
        public long AccountId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int AllowTest { get; set; }
        public long TotalBaseMoney { get; set; }
        public long TotalDiscountMoney { get; set; }
        public long VourcherDiscount { get; set; }
        public long ShipMoney { get; set; }
        public long TotalMoney { get; set; }
        public long? PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; }
        public DateTime? TimeCreate { get; set; }
        public DateTime? TimeUpdate { get; set; }
        public string TempBarcodes { get; set; }
        public string TempNumbers { get; set; }
        public int VourcherId { get; set; }
    }
}
