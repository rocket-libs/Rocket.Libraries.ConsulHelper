using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Rocket.Libraries.ConsulHelper.Models;
using Rocket.Libraries.ConsulHelper.Services.ConsulRegistryReading;
using Rocket.Libraries.ConsulHelper.Services.ConsulRegistryWriting;

namespace Rocket.Libraries.ConsulHelper.Convenience
{
    public static class Bootstrapper
    {
        internal const byte MaxRegistrationAttempts = 10;
        public static IServiceCollection AddConsulHelper (
            this IServiceCollection services,
            IConfiguration configuration,
            string settingsSectionName = "ServiceDiscovery")
        {
            services.AddHttpClient<IConsulRegistryWriter, ConsulRegistryWriter> ()
                .AddPolicyHandler (GetRetryPolicy ())
                .AddPolicyHandler (GetCircuitBreakerPolicy ());
            services.Configure<ConsulRegistrationSettings> (configuration.GetSection (settingsSectionName));
            services.AddSingleton<IConsulRegistryReader, ConsulRegistryReader>();
            services.AddHostedService<Runner> ();
            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy ()
        {
            Random jitterer = new Random ();
            return HttpPolicyExtensions
                .HandleTransientHttpError ()
                .OrResult (msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync (MaxRegistrationAttempts,
                    retryAttempt => TimeSpan.FromSeconds (Math.Pow (2, retryAttempt)) +
                    TimeSpan.FromMilliseconds (jitterer.Next (0, 100)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy ()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError ()
                .CircuitBreakerAsync (5, TimeSpan.FromSeconds (30));
        }
    }
}