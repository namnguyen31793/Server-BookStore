using BookStoreCMS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreCMS.Utils
{
    public static class CONFIG
    {
        public static void Initialize(string secretCookieKey, string Baselink, EmailConfiguration emailConfig)
        {
            CONFIG.SecretTokenKey = secretCookieKey;
            CONFIG.BaseLink = Baselink;
            CONFIG.EmailConfig = emailConfig;
        }

        public static string SecretTokenKey = "";

        public static string BaseLink = "";
        public static string FunctionValidateEmail = "v1/Validate/Email?key=";

        public static EmailConfiguration EmailConfig;
    }
}
