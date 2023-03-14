using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class LikeBookModel
    {
        public long AccountId { get; set; }
        public string Barcode { get; set; }
        public string BookName { get; set; }
        public string ActionTime { get; set; } 
        public bool Status { get; set; } 
    }
}
