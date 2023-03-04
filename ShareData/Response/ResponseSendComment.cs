using ShareData.DB.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class ResponseSendComment
    {
        public RateCommentObject comment { get; set; }
        public RateCountModel rateStar { get; set; }
    }
}
