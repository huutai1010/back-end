using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class AgoraChatMessageRequest
    {
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public List<string> To { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("body")]
        public AgoraChatMessageTextBody Body { get; set; }

        
    }

    public class AgoraChatMessageTextBody
    {
        [JsonProperty("msg")]
        public string Content { get; set; }
    }

    
}
