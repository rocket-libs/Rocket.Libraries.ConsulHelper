using Rocket.Libraries.ConsulHelper.Services.ConsulRegistryReading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.ConsulHelper.Tests.Services.ConsulRegistryReading
{
    public class ConsulRegistryReaderTests
    {
        [Fact]
        public async Task FetchAllServiceNames()
        {
            var serviceNames = await new ConsulRegistryReader()
        }
    }
}