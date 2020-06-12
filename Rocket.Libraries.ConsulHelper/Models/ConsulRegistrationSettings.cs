using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rocket.Libraries.ConsulHelper.Convenience;
using Rocket.Libraries.ConsulHelper.Services;

namespace Rocket.Libraries.ConsulHelper.Models
{
    public class ConsulRegistrationSettings
    {
        private string id;
        private string address;

        [JsonProperty ("ID")]
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty (id))
                {
                    return Name;
                }
                else
                {
                    return id;
                }
            }

            set => id = value;
        }

        [JsonProperty ("Name")]
        public string Name { get; set; }

        [JsonProperty ("Tags")]
        public List<string> Tags { get; set; }

        [JsonIgnore]
        public string Address
        {
            get
            {
                if (string.IsNullOrEmpty (address))
                {
                    address = $"http://{IpAddressProvider.IpAddress}";
                }
                return address;
            }
            set => address = value;
        }

        [JsonProperty ("Address")]
        public Uri AddressUri => new Uri (Address);

        [JsonProperty ("Port")]
        public long Port { get; set; }

        [JsonProperty ("EnableTagOverride")]
        public bool EnableTagOverride { get; set; }

        [JsonProperty ("Check")]
        public ConsulCheck Check { get; set; }

        [JsonProperty ("Weights")]
        public Weights Weights { get; set; }

        [JsonIgnore]
        public string ConsulUrl { get; set; }

        [JsonIgnore]
        public string AddressWithoutTailingSlash => UrlHelper.RemoveTrailingSlashIfRequired (Address);
    }

}