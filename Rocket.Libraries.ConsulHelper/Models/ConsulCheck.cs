using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rocket.Libraries.ConsulHelper.Models
{
    public class ConsulCheck
    {
        private List<string> args;

        [JsonProperty ("DeregisterCriticalServiceAfter")]
        public string DeregisterCriticalServiceAfter { get; set; }

        [JsonIgnore]
        public string HttpHealth { get; set; }

        [JsonProperty ("Args")]
        public List<string> Args
        {
            get
            {
                if (args == default)
                {
                    if (!string.IsNullOrEmpty (HttpHealth))
                    {
                        args = new List<string> { HttpHealth };
                    }
                }
                return args;
            }
        }

        [JsonProperty ("Interval")]
        public string Interval { get; set; }

        [JsonProperty ("Timeout")]
        public string Timeout { get; set; }
    }
}