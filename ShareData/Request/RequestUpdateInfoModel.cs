using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestUpdateInfoModel
    {
        public string Nickname { get; set; }    //ho ten (50 ky tu)
        public string PhoneNumber { get; set; }  //50 max
        public DateTime? BirthDay { get; set; }    //yyyy-mm-dd
        public string Adress { get; set; }    //500 max
        public string AvataLink { get; set; }    
        public bool? Sex { get; set; }    
    }
}
