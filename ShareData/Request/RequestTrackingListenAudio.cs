using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareData.Request
{
    public class RequestTrackingListenAudio
    {
        [JsonProperty("Id")]
        public long AccountId { get; set; }
        [JsonProperty("B")]
        public string Barcode { get; set; }
        [JsonProperty("C")]
        public long Count { get; set; }
    }
}
