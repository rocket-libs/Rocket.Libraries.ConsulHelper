using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Rocket.Libraries.ConsulHelper.Models;
using Rocket.Libraries.ConsulHelper.Services.ConsulRegistryWriting;

namespace Rocket.Libraries.ConsulHelper.Convenience
{
    public static class Bootstrapper
    {
        public static void ConfigureConsulHelper (
            this IServiceCollection services,
            IConfiguration configuration,
            string settingsSectionName = "RegistrationDescription")
        {
            services.AddHttpClient<IConsulHttpClientWrapper, ConsulHttpClientWrapper> ()
                .AddPolicyHandler (GetRetryPolicy ())
                .AddPolicyHandler (GetCircuitBreakerPolicy ());
            services.Configure<ConsulRegistrationSettings> (configuration.GetSection (settingsSectionName));
            services.AddHostedService<ConsulRegistryWriter> ();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy ()
        {
            Random jitterer = new Random ();
            return HttpPolicyExtensions
                .HandleTransientHttpError ()
                .OrResult (msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync (6,
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