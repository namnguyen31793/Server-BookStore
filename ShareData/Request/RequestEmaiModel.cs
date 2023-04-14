using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestEmaiModel
    {
        public long MailId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
    }
}
