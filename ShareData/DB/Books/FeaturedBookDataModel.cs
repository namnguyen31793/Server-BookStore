using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class FeaturedBookDataModel
    {
        public long FeatureId { get; set; }
        public int FeatureType { get; set; }
        public string Barcode { get; set; }
    }
    public class ReprintBookDataModel
    {
        public string Barcode { get; set; }
    }
    public class TopLike
    {
        public string Barcode { get; set; }
        public long TotalLike { get; set; }
    }
}
