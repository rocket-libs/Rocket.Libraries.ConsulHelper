using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rocket.Libraries.ConsulHelper.Convenience
{
    /// <summary>
    /// Provides an available port number on which your service may run.
    /// </summary>
    public static class PortNumberProvider
    {
        private static int _port;

        /// <summary>
        /// Gets or sets a port number.
        /// Reading the port before setting, causes the provider to generate a random port number.
        /// Note: This property will return the same port number per lifetime of the service, unless you call the method <see cref="Reset"/> which forces generation of a new port number
        /// when you next read the property.
        /// </summary>
        public static int Port
        {
            get
            {
                if (_port == default)
                {
                    _port = GetRandomPort();
                }

                return _port;
            }

            set => _port = value;
        }

        /// <summary>
        /// Resets the port number to allow generation of a new random one.
        /// </summary>
        public static void Reset()
        {
            _port = default;
        }

        private static int GetRandomPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}