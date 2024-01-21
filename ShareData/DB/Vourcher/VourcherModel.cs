using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.DB.Vourcher
{
    public class VourcherModel
    {
        public int VourcherId { get; set; }
        public string VourcherName { get; set; }
        public int VourcherType { get; set; }   //-1 distcount, 2- link
        public int CountUse { get; set; }   //-1 vo han
        public string VourcherReward { get; set; }  //number or link
        public string VourcherDescription { get; set; } //giai thich
        public bool Status { get; set; } //giai thich
    }
    public class VourcherModelVer2
    {
        public int VourcherId { get; set; }
        public string VourcherName { get; set; }
        public int VourcherType { get; set; }   //-1 distcount, 2- link
        public int CountUse { get; set; }   //-1 vo han
        public string VourcherReward { get; set; }  //number or link
        public string VourcherDescription { get; set; } //giai thich
        public string thumbnail { get; set; } 
        public string Targets { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; } //giai thich
    }
    public class VourcherCountUse
    {
        public string VourcherId { get; set; }
        public string VourcherName { get; set; }
        public long CountUse { get; set; }  
    }
}
