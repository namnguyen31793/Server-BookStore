using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ShareData.DB.Books
{
    public class BookColorConfig
    {
        public int ColorId {get; set; }
        public string Cover { get; set; }
        public string Media { get; set; }
        public bool Status { get; set; }
    }
}
