using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Order
{
    public class AdressModel
    {
        public string AdrCity { get; set; }
        public int CodeCity { get; set; }
        public string AdrdDistrict { get; set; }
        public int CodeDistrict { get; set; }
        public string AdrWard { get; set; }
        public string CodeWard { get; set; }
        public string AdrDetail { get; set; }
    }
}
