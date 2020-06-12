namespace Rocket.Libraries.ConsulHelper.Configuration
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using Rocket.Libraries.ConsulHelper.Convenience;
    using Rocket.Libraries.ConsulHelper.Models;

    /// <summary>
    /// This class enables easy inject of settings that'll be used in registering your service and finding Consul
    /// </summary>
    public class ConsulSettingsInjector : IDisposable
    {
        private HealthCheckConfigBuilder healthCheckConfigBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulSettingsInjector"/> class.
        /// </summary>
        /// <param name="serviceName">The name of your service.</param>
        public ConsulSettingsInjector(string serviceName)
        {
            ServiceName = serviceName;
        }

        /// <summary>
        /// Gets or sets the name of your service.
        /// </summary>
        private string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the port your service is listening on.
        /// </summary>
        private int ServicePort { get; set; }

        /// <summary>
        /// Gets or sets the url where Consul is reacheable on.
        /// </summary>
        private string ConsulUrl { get; set; }

        /// <summary>
        /// Gets or sets the base url of your service;
        /// </summary>
        private string ServiceBaseUrl { get; set; }

        /// <summary>
        /// Allows you to set a custom port on which your service will be listening on.
        /// If you don't set one, a random port number will be generated for you.
        /// </summary>
        /// <param name="servicePort">Port your service is listening on.</param>
        /// <returns>This instance of the <see cref="ConsulSettingsInjector"/>.</returns>
        public ConsulSettingsInjector UseCustomServicePort(int servicePort)
        {
            FailIfDisposed();
            ServicePort = servicePort;
            return this;
        }

        /// <summary>
        /// Allows you to set a custom url for where Consul is reachable.
        /// If you don't set a value, config defaults to 'http://localhost:8500'.
        /// </summary>
        /// <param name="consulUrl">Url that points to where Consul is reachable.</param>
        /// <returns>This instance of the <see cref="ConsulSettingsInjector"/>.</returns>
        public ConsulSettingsInjector UseConsulAtCustomUrl(string consulUrl)
        {
            FailIfDisposed();
            ConsulUrl = consulUrl;
            return this;
        }

        /// <summary>
        /// Allows you to set a custom base url for your service.
        /// If none is set, then the default 'http://localhost' is used.
        /// </summary>
        /// <param name="serviceBaseUrl">The base url of your service.</param>
        /// <returns>This instance of the <see cref="ConsulSettingsInjector"/>.</returns>
        public ConsulSettingsInjector UseCustomServiceBaseUrl(string serviceBaseUrl)
        {
            FailIfDisposed();
            ServiceBaseUrl = serviceBaseUrl;
            return this;
        }

        /// <summary>
        /// Allows you to set your own endpoint that consul can periodically ping to establish health of your service.
        /// If one is not set, then the endpoint 'api/consul-helper/service-health' is used.
        /// </summary>
        /// <param name="httpHealthRelativeUrl">Url to your health check endpoint.</param>
        /// <returns>This instance of the <see cref="ConsulSettingsInjector"/>.</returns>
        public HealthCheckConfigBuilder UseHealthCheckConfigBuilder(string httpHealthRelativeUrl)
        {
            FailIfDisposed();
            if (healthCheckConfigBuilder == null)
            {
                healthCheckConfigBuilder = new HealthCheckConfigBuilder(this, httpHealthRelativeUrl);
            }

            return healthCheckConfigBuilder;
        }

        /// <summary>
        /// Injects the settings into an instance of the <see cref="ConsulRegistrationSettings" />
        /// Will throw an exception if a null object is supplied.
        /// </summary>
        /// <param name="consulRegistrationSettings">An instance of <see cref="ConsulRegistrationSettings" /> into which settings are injected.</param>
        public void Inject(ConsulRegistrationSettings consulRegistrationSettings)
        {
            if (consulRegistrationSettings == null)
            {
                throw new Exception($"Parameter {nameof(consulRegistrationSettings)} must be instantiated outside of the inject method");
            }

            if (string.IsNullOrEmpty(ServiceName))
            {
                throw new Exception($"{nameof(ServiceName)} was not provided. This value is required");
            }

            if (ServicePort == 0)
            {
                UseCustomServicePort(PortNumberProvider.Port);
            }

            if (string.IsNullOrEmpty(ConsulUrl))
            {
                UseConsulAtCustomUrl("http://localhost:8500");
            }

            if (string.IsNullOrEmpty(ServiceBaseUrl))
            {
                UseCustomServiceBaseUrl("http://localhost");
            }

            consulRegistrationSettings.Address = ServiceBaseUrl;
            consulRegistrationSettings.ConsulUrl = ConsulUrl;
            consulRegistrationSettings.Name = ServiceName;
            consulRegistrationSettings.Port = ServicePort;
            if (healthCheckConfigBuilder == null)
            {
                healthCheckConfigBuilder = new HealthCheckConfigBuilder(this, "api/consul-helper/health");
            }

            healthCheckConfigBuilder.BuildHealthCheck();

            consulRegistrationSettings.Check = new ConsulCheck
            {
                HttpHealth = healthCheckConfigBuilder.HttpHealthRelativeUrl,
                DeregisterCriticalServiceAfter = healthCheckConfigBuilder.DeregisterCriticalServiceAfter,
                Interval = healthCheckConfigBuilder.Interval,
            };
        }

        private void FailIfDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(ConsulSettingsInjector));
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private readonly ConsulRegistrationSettings consulConfig;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    healthCheckConfigBuilder?.Dispose();
                    healthCheckConfigBuilder = null;
                    ServiceName = string.Empty;
                    ServicePort = default;
                    ConsulUrl = string.Empty;
                    ServiceBaseUrl = string.Empty;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConsulConfigBuilder()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}