using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class FeaturedBookConfigModel
    {
        public int FeatureType { get; set; }
        public string FeatureName { get; set; }
        public string LinkIcon { get; set; }
        public bool Status { get; set; }
    }
}
