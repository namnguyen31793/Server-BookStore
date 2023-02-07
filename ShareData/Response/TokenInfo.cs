using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Response
{
    public class TokenInfo
    {
        public long AccountId { get; set; }
        public string Access_token { get; set; }
        public string Refresh_token { get; set; }
    }
    public class TokenInfoCms
    {
        public int Role { get; set; }
        public long AccountId { get; set; }
        public string Access_token { get; set; }
        public string Refresh_token { get; set; }
    }
}
