using ShareData.DB.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class ResponseBuyBook
    {
        public SimpleBookModel modelBook { get; set; }
        public long CurrentPoint { get; set; }
        public long CurrentVip { get; set; }
        public string VipName { get; set; }
        public bool levelUp { get; set; }   //true call get mail
    }
}
