using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.API
{
    public class RequestApiModel<T>
    {
        public long ApiRequestCode { get; set; }
        public long Status { get; set; }
        public T PlayerRequestInfo { get; set; }
        public T DataRequest { get; set; }
    }

    public class RequestApiClientModel<T>
    {
        public long ApiRequestCode { get; set; }
        public long Status { get; set; }
        public T DataRequest { get; set; }
    }
}
