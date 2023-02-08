using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class AddBookModel
    {
        public string Barcode { get; set; }
        public string ImageLink { get; set; }
        public string BookName { get; set; }
        public int AuthorId { get; set; }
        public long AmountBase { get; set; }
        public long AmountSale { get; set; }
        public string TrialReadLink { get; set; }
        public string AudioLink { get; set; }
        public string KeyLink { get; set; }
        public long NumberPage { get; set; }
        public string Tags { get; set; }
        public string ContentBook { get; set; }
        public string RelatedBooks { get; set; }
        public bool Status { get; set; }
        public int ColorId { get; set; }
    }
}
