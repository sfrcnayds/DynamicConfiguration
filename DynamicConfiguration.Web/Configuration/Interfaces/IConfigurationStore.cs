using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Web.Configuration.Models;

namespace DynamicConfiguration.Web.Configuration.Interfaces
{
    public interface IConfigurationStore
    {
        Task<List<ConfigurationItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ConfigurationItem> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(ConfigurationItem item, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(string id, ConfigurationItem item, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}