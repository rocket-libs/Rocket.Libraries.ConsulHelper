using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rocket.Libraries.ConsulHelper.Convenience;
using Rocket.Libraries.ConsulHelper.Models;

namespace Rocket.Libraries.ConsulHelper.Services.ConsulRegistryWriting
{
    

    public class ConsulRegistryWriter : IConsulRegistryWriter
    {

        private ConsulRegistrationSettings _serviceSettings;

        private ILoggerFactory _loggerFactory;

        private ILogger _logger;

        private readonly HttpClient httpClient;

        /*/// <summary>
        /// Initializes a new instance of the <see cref="ConsulHttpWrapper /> class
        /// </summary>
        /// <param name="serviceSettingsOpts">Information about the service to be registered.</param>
        /// <param name="loggerFactory">An instance of ILoggerFactory fed in via Dependancy Injection that's used to generate a logger.</param>
        /// <param name="httpClient">An instance of HttpClient object to facilitate communication with Consul.</param>*/
        public ConsulRegistryWriter (HttpClient httpClient, IOptions<ConsulRegistrationSettings> serviceSettingsOpts, ILoggerFactory loggerFactory)
        {
            this.httpClient = httpClient;
            _serviceSettings = serviceSettingsOpts.Value;
            _loggerFactory = loggerFactory;
        }
        private ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = _loggerFactory.CreateLogger<Runner> ();
                }

                return _logger;
            }
        }

        /// <summary>
        /// Writes the service's information to Consul.
        /// </summary>
        /// <returns>Nothing.</returns>
        public async Task RegisterAsync ()
        {
            try
            {
                AutoDetectPortIfRequired ();
                Logger.LogNoisyInformation ($"Registering self to consul using the IP Address '{_serviceSettings.Address}'");
                var payload = JsonConvert.SerializeObject (_serviceSettings);
                Logger.LogNoisyInformation ($"Attempting Registration of {_serviceSettings.Name} to Consul at {_serviceSettings.ConsulUrl}");
                Logger.LogNoisyInformation ($"Service Address: {_serviceSettings.Address}");
                Logger.LogNoisyInformation ($"Service Port: {_serviceSettings.Port}");
                Logger.LogNoisyInformation ($"Full Payload: {payload}");
                LogAboutHealthCheck ();
                var request = HttpRequestMessageProvider.Get (HttpMethod.Put, _serviceSettings.ConsulUrl, "v1/agent/service/register");
                request.Content = new StringContent (payload, System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync (request);
                LogResponse (response);
            }
            catch (Exception e)
            {
                Logger.LogNoisyError (e, $"Error occured registering service {_serviceSettings.Name} with Consul");
                Logger.LogNoisyWarning ($"Service {_serviceSettings.Name} failed to register with Consul. This may impact performance of other services as they won't be able to locate it");
            }
        }

        private void AutoDetectPortIfRequired ()
        {
            var hasPort = _serviceSettings.Port != default;
            if (!hasPort)
            {
                _serviceSettings.Port = PortNumberProvider.Port;
                Logger.LogNoisyWarning ($"Registering self to consul using the the auto-detected Port '{_serviceSettings.Port}'");
            }
        }

        private void LogAboutHealthCheck ()
        {
            var healthCheckMissing = string.IsNullOrEmpty (_serviceSettings.Check?.HttpHealth);
            if (healthCheckMissing)
            {
                Logger.LogNoisyWarning ($"No health-check specified. This isn't necessarily catastrophic but because of this, Consul will continue serving up this service's url even when the service is not available");
            }
            else
            {
                Logger.LogNoisyInformation ($"Health check for service shall be registered");
            }
        }

        private void LogResponse (HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Logger.LogNoisyInformation ($"Service Registration With Consul Succeeded");
            }
            else
            {
                Logger.LogNoisyError (null, $"Service Registration With Consul Failed With Status Code {response.StatusCode}");
            }
        }
    }
}