using ShareData.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.API
{
    public class ResponseApiModel<T>
    {
        private string _message;

        public long Status { get; set; }

        public string Messenger
        {
            get
            {
                if (string.IsNullOrEmpty(_message)) return "";
                return _message;
            }
            set { _message = value; }
        }

        public T DataResponse { get; set; }
    }
}
