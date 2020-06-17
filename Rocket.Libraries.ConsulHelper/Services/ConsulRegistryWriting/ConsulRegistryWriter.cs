namespace Rocket.Libraries.ConsulHelper.Services.ConsulRegistryWriting
{
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// This class contains functionality to allow writing of service information to Consul.
    /// </summary>
    public class ConsulRegistryWriter : IHostedService, IConsulRegistryWriter
    {
        private readonly IConsulHttpClientWrapper consulHttpClientWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulRegistryWriter"/> class.
        /// </summary>
        public ConsulRegistryWriter (IConsulHttpClientWrapper consulHttpClientWrapper)
        {
            this.consulHttpClientWrapper = consulHttpClientWrapper;
        }

        public async Task RegisterAsync ()
        {
            await consulHttpClientWrapper.RegisterAsync ();
        }

        /// <inheritdoc/>
        public async Task StartAsync (CancellationToken cancellationToken)
        {
            await consulHttpClientWrapper.RegisterAsync ();
        }

        /// <inheritdoc/>
        public Task StopAsync (CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}