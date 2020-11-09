using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rocket.Libraries.ConsulHelper.Convenience;
using Rocket.Libraries.ConsulHelper.Services;

namespace Rocket.Libraries.ConsulHelper.Models
{
    /// <summary>
    /// Information required by the library to allow it to interact successfully with Consul.
    /// </summary>
    public class ConsulRegistrationSettings
    {
        private string id;
        

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

        /// <summary>
        /// Gets or sets the name of your service.
        /// </summary>
        [JsonProperty ("Name")]
        public string Name { get; set; }

        [JsonProperty ("Tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the base url of your service.
        /// </summary>
        [JsonProperty ("Address")]
        public string Address
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the base url of your service.
        /// </summary>
        
        internal Uri AddressUri 
        {
            get 
            {
                if(string.IsNullOrEmpty(Address))
                {
                    return default;
                }
                else
                {
                    return new Uri(Address);
                }
            }
        }

        /// <summary>
        /// Gets or sets the url your service is listening on.
        /// </summary>
        [JsonProperty ("Port")]
        public long Port { get; set; }

        [JsonProperty ("EnableTagOverride")]
        public bool EnableTagOverride { get; set; }

        /// <summary>
        /// Gets or sets settings that control health checks on your service.
        /// </summary>
        /// <remarks>Not yet implemented</remarks>
        [JsonProperty ("Check")]
        public ConsulCheck Check { get; set; }

        /// <summary>
        /// Gets or sets information about the weights assigned to services to allow load balancing and determining health of services.
        /// </summary>
        [JsonProperty ("Weights")]
        public Weights Weights { get; set; }

        /// <summary>
        /// Gets or sets the full url (including port) to Consul.
        /// </summary>
        [JsonIgnore]
        public string ConsulUrl { get; set; }

        [JsonIgnore]
        internal string AddressWithoutTailingSlash => UrlHelper.RemoveTrailingSlashIfRequired (Address);
    }

}