using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.API
{
    public class InfoLiveStreamModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string LinkImage { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }
        public string Channel { get; set; }
    }
}
