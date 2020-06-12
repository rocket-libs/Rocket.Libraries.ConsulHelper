namespace Rocket.Libraries.ConsulHelper.Convenience
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Convenience class to help obtain machine's IP Address.
    /// </summary>
    public static class IpAddressProvider
    {
        /// <summary>
        /// Gets the first IPV4 address of the machine the assembly is executing on.
        /// </summary>
        public static string IpAddress
        {
            get
            {
                string hostName = Dns.GetHostName();
                var ipAddresses = Dns.GetHostEntry(hostName).AddressList;

                var firstIpV4Address = ipAddresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                if (firstIpV4Address == default)
                {
                    throw new Exception("Could not determine this machine's IPV4 address");
                }

                return firstIpV4Address.ToString();
            }
        }
    }
}