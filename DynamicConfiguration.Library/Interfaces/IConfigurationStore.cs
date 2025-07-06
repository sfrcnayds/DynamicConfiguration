using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Library.Models;

namespace DynamicConfiguration.Library.Interfaces
{
    public interface IConfigurationStore
    {
        public Task<List<ConfigurationItem>> GetActiveConfigs(string applicationName,
            CancellationToken cancellationToken = default);
    }
}