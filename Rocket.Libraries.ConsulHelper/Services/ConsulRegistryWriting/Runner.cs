namespace Rocket.Libraries.ConsulHelper.Services.ConsulRegistryWriting
{
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// This class initiates the functionality of the library by causing your service to be registered.
    /// </summary>
    public class Runner : IHostedService
    {
        private readonly IConsulRegistryWriter consulRegistryWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Runner"/> class.
        /// </summary>
        public Runner (IConsulRegistryWriter consulRegistryWriter)
        {
            this.consulRegistryWriter = consulRegistryWriter;
        }

         /// <summary>
        /// Triggers the writing of the service's information to Consul.
        /// </summary>
        /// <returns>Nothing.</returns>
        public async Task RegisterAsync ()
        {
            await consulRegistryWriter.RegisterAsync ();
        }

        /// <inheritdoc/>
        public async Task StartAsync (CancellationToken cancellationToken)
        {
            await consulRegistryWriter.RegisterAsync ();
        }

        /// <inheritdoc/>
        public Task StopAsync (CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}