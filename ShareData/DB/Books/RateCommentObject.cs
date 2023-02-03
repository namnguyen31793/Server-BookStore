using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class RateCommentObject
    {
        public long AccountId { get; set; }
        public string Barcode { get; set; }
        public int StarRate { get; set; }
        public string Comment { get; set; }
        public string ActionTime { get; set; }
        public string NickName { get; set; }
    }
}
