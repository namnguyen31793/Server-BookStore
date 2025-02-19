﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class ResponseGhnModel<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class ResponseGhnData {
        public string district_encode { get; set; }
        public string expected_delivery_time { get; set; }
        public string order_code { get; set; }
        public string sort_code { get; set; }
        public string total_fee { get; set; }
        public string trans_type { get; set; }
    }

    public class ServiceId
    {
        public long service_id { get; set; }
        public string short_name { get; set; }
        public long service_type_id { get; set; }
    }

    public class ResponseNhanhData
    {
        public int orderId { get; set; }//int ID đơn hàng trên Nhanh.vn
        public int shipFee { get; set; }// Phí vận chuyển
        public int codFee { get; set; } // Phí thu tiền hộ
        public int declaredFee { get; set; }// Phí bảo hiểm 
        public string carrierCode { get; set; }// Mã vận đơn
    }
}
