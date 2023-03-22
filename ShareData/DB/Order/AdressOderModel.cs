using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Order
{
    public class AdressOderModel
    {
        public string city { get; set; }
        public int cityCode { get; set; }
        public int districtCode { get; set; }
        public int wardCode { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        public string detail { get; set; }
    }
}
