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

    public class RequestGetFeeNhanhModel
    {
        public string fromCityName { get; set; }    //Tên thành phố của kho gửi hàng (Lấy từ /api/shipping/location).
        public string fromDistrictName { get; set; }   //Tên quận huyện của kho gửi hàng 
        public string toCityName { get; set; }   //Tên thành phố của khách nhận hàng 
        public string toDistrictName { get; set; } //Tên quận huyện của khách nhận hàng
        public int codMoney { get; set; } //Giá trị tiền cần thu hộ của đơn hàng
        public int shippingWeight { get; set; } //Tổng trọng lượng các sản phẩm của đơn hàng tính bằng gram
        public int[] carrierIds { get; set; } //Dùng để giới hạn các hãng vận chuyển muốn dùng (Lấy từ /api/shipping/carrier). VD: [5,7,8,9]
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class RequestProcessOrderNhanhModel
    {
        public long OrderId { get; set; }
        public long DepotId { get; set; }   //id kho hang tren Nhanh (Lấy từ /api/store/depot)
        public string Type { get; set; }   //Loại đơn hàng, giá trị có thể là: “Shipping” (Chuyển hàng) hoặc “Shopping” (Khách tới mua tại cửa hàng) “PreOrder”(Khách đặt hàng trước). Giá trị mặc định là Shipping
        public int AllowTest { get; set; }//1-cho xem hàng, k thử, 2-cho thử, 3-không xem hàng, 4-cho xem, k thu ship, mac dinh 3
        public string PrivateDescription { get; set; } //note cua van hanh
        public int carrierId { get; set; }  //id hãng vận chuyển (Lấy từ /api/shipping/fee)
        public int carrierServiceId { get; set; }   //dịch vụ vận chuyển (Lấy từ /api/shipping/fee)
        public int customerShipFee { get; set; }
    }
}
