using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class AuthorInfoModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorBirday { get; set; } //format time to string
        public string AuthorAdress { get; set; }
        public string AuthorIntroduction { get; set; }
    }
}
