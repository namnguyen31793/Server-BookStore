using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestCreateOrderModel
    {
        public long CustomerId { get; set; }
        public string Type { get; set; }//--Shipping, Shopping, PreOrder : defaut Shipping
        public string Description { get; set; } // ghi chu khach hang
        public string Barcodes { get; set; } //list barcode split ',' : VD: "180123312,31241241"
        public string Numbers { get; set; } //list count book split ',' : VD: "1,2"
        public int VourcherId { get; set; } //mặc đinh k có  = 0
        public string PaymentMethod { get; set; } //COD hoặc STORE, mặc định COD
    }
}
