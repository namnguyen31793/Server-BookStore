using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestTrackingFindBook
    {
        [JsonProperty("Id")]
        public long AccountId { get; set; }
        [JsonProperty("A")]
        public string Action { get; set; }
        [JsonProperty("B")]
        public string Barcode { get; set; }
        [JsonProperty("C")]
        public long Count { get; set; }
    }
}
