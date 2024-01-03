using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AgoraIO.Common
{
    public class AddUserAgora
    {
        [JsonProperty("username")]
        public string UserName { get; set;}
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
    }
}
