using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestCreateOrderCmsModel
    {
        public long AccountId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Type { get; set; }//--Shipping, Shopping, PreOrder : defaut Shipping
        public string Description { get; set; } // ghi chu khach hang
        public string Barcodes { get; set; } //list barcode split ',' : VD: "180123312,31241241"
        public string Numbers { get; set; } //list count book split ',' : VD: "1,2"
        public string PaymentMethod { get; set; } //COD hoặc STORE, mặc định COD
        public long ShipMoney { get; set; }
        public long TotalDiscountMoney { get; set; }    // gia giam tren bia
        public long VourcherMoney { get; set; }    // tien giam gia
    }
}
