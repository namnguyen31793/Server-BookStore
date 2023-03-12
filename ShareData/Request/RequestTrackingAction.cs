using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestTrackingAction
    {
        [JsonProperty("Id")]
        public long AccountId { get; set; }
        [JsonProperty("A")]
        public string Action { get; set; }
        [JsonProperty("C")]
        public long Count { get; set; }
    }
    public class RequestTrackingAction2
    {
        [JsonProperty("Id")]
        public long AccountId { get; set; }
        [JsonProperty("D")]
        public int Deep { get; set; }
        [JsonProperty("A")]
        public string Action { get; set; }
        [JsonProperty("C")]
        public long Count { get; set; }
    }
}
