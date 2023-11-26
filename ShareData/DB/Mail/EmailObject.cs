using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Mail
{
    public class EmailObject
    {
        public long MailId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long CountUse { get; set; }
        public DateTime? LastActionTime { get; set; }
        public int Status { get; set; }
    }
}
