using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Books
{
    public class DownloadBookModel
    {
        public string Barcode { get; set; }
        public string AudioLink { get; set; }
        public string KeyLink { get; set; }
    }
}
