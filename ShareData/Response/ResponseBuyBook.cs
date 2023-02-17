using ShareData.DB.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class ResponseBuyBook
    {
        public SimpleBookModel modelBook { get; set; }
        public ResponseMemberVip modelMember { get; set; }
        public long pointBook { get; set; }   
        public bool levelUp { get; set; }   //true call get mail
    }
}
