using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsSystem.SocialNetwork
{
    public class FacebookApiResponse
    {
        public List<DataFbApp> data { get; set; }
        public Paging paging { get; set; }
    }
    public class Paging
    {
        public Cursors cursors { get; set; }
    }
    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }
    public class DataFbApp
    {
        public string id { get; set; }
        public FbApp app { get; set; }
    }
    public class FbApp
    {
        public string category { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }
}
