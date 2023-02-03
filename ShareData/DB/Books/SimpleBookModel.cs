using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class SimpleBookModel
    {
        public string Barcode { get; set; }
        public string ImageLink { get; set; }
        public string BookName { get; set; }
        public string Tags { get; set; }
    }
}
