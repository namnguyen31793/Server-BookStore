using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestUpdateInfoModel
    {
        public string Nickname { get; set; }    //ho ten (50 ky tu)
        public string AvatarId { get; set; }    
        public string PhoneNumber { get; set; }  //50 max
        public string BirthDay { get; set; }    //yyyy-mm-dd
        public string Adress { get; set; }    //500 max
    }
}
