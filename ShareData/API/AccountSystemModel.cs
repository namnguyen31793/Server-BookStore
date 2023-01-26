using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.API
{
    public class AccountSystemModel
    {
        public string AccountID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public long AccountBalance { get; set; }
        public string Cookie { get; set; }
    }
    public class AccountServerSystemModel
    {
        public string AccountID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
    }
}
