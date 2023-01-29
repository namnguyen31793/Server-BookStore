using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Utils
{
    public static class CONFIG
    {
        public static void Initialize(string secretCookieKey)
        {
            CONFIG.SecretTokenKey = secretCookieKey;
        }

        public static string SecretTokenKey = "";
    }
}
