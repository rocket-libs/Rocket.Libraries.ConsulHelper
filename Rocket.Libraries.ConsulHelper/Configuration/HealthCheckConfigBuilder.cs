using System;

namespace Rocket.Libraries.ConsulHelper.Configuration
{
    /// <summary>
    /// This class allows configuration of Consul's service health checking
    /// </summary>
    public class HealthCheckConfigBuilder : IDisposable
    {
        private readonly ConsulSettingsInjector configBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckConfigBuilder"/> class.
        /// </summary>
        /// <param name="configInjector">Owner instance of <see cref="ConsulSettingsInjector"></see>/></param>
        /// <param name="httpHealthRelativeUrl">Relative url to endpoint for health checks</param>
        internal HealthCheckConfigBuilder(ConsulSettingsInjector configInjector, string httpHealthRelativeUrl)
        {
            this.configBuilder = configInjector;
            HttpHealthRelativeUrl = httpHealthRelativeUrl;
        }

        /// <summary>
        /// Gets relative url to endpoint for health checks
        /// </summary>
        public string HttpHealthRelativeUrl { get; private set; }

        /// <summary>
        /// Gets duration after which a service should be removed from Consul, if unhealthy
        /// </summary>
        public string DeregisterCriticalServiceAfter { get; private set; }

        /// <summary>
        /// Gets the interval after which Consul should ping the service for health
        /// </summary>
        public string Interval { get; private set; }

        /// <summary>
        /// Allows you to set your own custom duration for which unhealthy services should remain in Consul's registry.
        /// If not called, then <paramref name="duration"/> is set to 1 and <paramref name="unit"/> is set to Minutes.
        /// </summary>
        /// <param name="duration">A numeric value indicating maginitude of time period.</param>
        /// <param name="unit">A unit indicating measurement of time period.</param>
        /// <returns>Instance of <see cref="HealthCheckConfigBuilder"/>.</returns>
        public HealthCheckConfigBuilder UseCustomDeregistrationTimeout(byte duration, TimeUnit unit)
        {
            FailIfDisposed();
            if (duration < 1)
            {
                throw new Exception($"Time durations small than 1 are not supported");
            }

            DeregisterCriticalServiceAfter = $"{duration}{GetUnit(unit)}";
            return this;
        }

        /// <summary>
        /// Allows you to set your own custom duration after which Consul should ping your service for health.
        /// If not call, then the defaults used are <paramref name="interval"/> = 10 and <paramref name="unit"/> = Seconds.
        /// </summary>
        /// <param name="interval">A numeric value indicating maginitude of time period.</param>
        /// <param name="unit">A unit indicating measurement of time period.</param>
        /// <returns>Instance of <see cref="HealthCheckConfigBuilder"/>.</returns>
        public HealthCheckConfigBuilder UseCustomDefaultHealthCheckInterval(byte interval, TimeUnit unit)
        {
            FailIfDisposed();
            if (interval < 1)
            {
                throw new Exception($"Time intervals small than 1 are not supported");
            }

            Interval = $"{interval}{GetUnit(unit)}";
            return this;
        }

        /// <summary>
        /// Assembles the  health check information into a nice litte package for consumption.
        /// </summary>
        /// <returns>Instance of <see cref="HealthCheckConfigBuilder"/>.</returns>
        internal ConsulSettingsInjector BuildHealthCheck()
        {
            FailIfDisposed();
            if (string.IsNullOrEmpty(HttpHealthRelativeUrl))
            {
                throw new Exception($"{nameof(HttpHealthRelativeUrl)} has not been specified");
            }

            if (string.IsNullOrEmpty(DeregisterCriticalServiceAfter))
            {
                UseCustomDeregistrationTimeout(1, TimeUnit.Minutes);
            }

            if (string.IsNullOrEmpty(Interval))
            {
                UseCustomDefaultHealthCheckInterval(10, TimeUnit.Seconds);
            }

            return configBuilder;
        }

        private string GetUnit(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "s";

                case TimeUnit.Minutes:
                    return "m";

                default:
                    throw new Exception($"Value '{unit}' is not supported as time unit");
            }
        }

        private void FailIfDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(HealthCheckConfigBuilder));
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpHealthRelativeUrl = string.Empty;
                    DeregisterCriticalServiceAfter = string.Empty;
                    Interval = string.Empty;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HealthCheckConfigBuilder()
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