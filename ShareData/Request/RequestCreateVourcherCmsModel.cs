using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestCreateVourcherCmsModel
    {
        public int VourcherId { get; set; }     // goi tin tao thi de rong
        public string VourcherName { get; set; }    //ten
        public int VourcherType { get; set; }   //-1 distcount %, 2- free ship
        public int CountUse { get; set; }   //-1 vo han, 1-n la so lan su dung
        public string VourcherReward { get; set; }  //number or link (de server parrse) VD: 3 type 1 -> 3% giam
        public string VourcherDescription { get; set; } //giai thich phan thuong
        public string thumbnail { get; set; }
        public string Targets { get; set; }     //list book same order
        public DateTime StartTime { get; set; } // time bat dau cho type 3
        public DateTime EndTime { get; set; }   // time ket thuc cho type 3
        public bool Status { get; set; } //active bat tat
    }
    public class RequestAddVourcherToUserCmsModel
    {
        public long AccountId { get; set; }
        public int VourcherId { get; set; }   
        public string VourcherName { get; set; } 
        public int CountUse { get; set; } 
    }
}
