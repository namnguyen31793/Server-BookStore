using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class BookBuyModel
    {
        public long AccountId { get; set; }
        public string Barcode { get; set; }
        public string BookName { get; set; }
    }
}
