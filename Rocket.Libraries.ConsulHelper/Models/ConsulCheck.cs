using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rocket.Libraries.ConsulHelper.Models
{
    public class ConsulCheck
    {
        private List<string> args;

        /// <summary>
        /// Gets or sets a value that specifies that checks associated with a service should deregister after this time. This is specified as a time duration with suffix like "10m". If a check is in the critical state for more than this configured value, then its associated service (and all of its associated checks) will automatically be deregistered. The minimum timeout is 1 minute.
        /// </summary>
        [JsonProperty ("DeregisterCriticalServiceAfter")]
        public string DeregisterCriticalServiceAfter { get; set; }

        /// <summary>
        /// Gets or sets relative url to the endpoint to be called for health checks while using the HTTP protocol on your service. MUST NOT include the base path of your service.
        /// </summary>
        /// <remarks>Not Yet Implemented</remarks>
        [JsonIgnore]
        public string HttpHealth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Not Yet Implemented</remarks>
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

        /// <summary>
        /// Gets or sets the the frequency at which to run health check.
        /// </summary>
        [JsonProperty ("Interval")]
        public string Interval { get; set; }

        [JsonProperty ("Timeout")]
        public string Timeout { get; set; }
    }
}