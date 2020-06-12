namespace Rocket.Libraries.ConsulHelper.Convenience
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Default convenience controller to minimize configs
    /// </summary>
    [Route("api/consul-helper/health")]
    [ApiController]
    public class ServiceHealthController
    {
        /// <summary>
        /// Simple http get endpoint that consul can use to check for reachability of service.
        /// </summary>
        /// <returns>Empty string.</returns>
        [HttpGet]
#pragma warning disable CA1822 // Mark members as static
        public string Index()
#pragma warning restore CA1822 // Mark members as static
        {
            return string.Empty;
        }
    }
}