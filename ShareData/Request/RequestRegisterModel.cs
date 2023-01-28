using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestRegisterModel
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    public class RequestAuthenSocial
    {
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
