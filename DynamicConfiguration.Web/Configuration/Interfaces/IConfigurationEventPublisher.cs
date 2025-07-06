using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Common.Events;

namespace DynamicConfiguration.Web.Configuration.Interfaces
{
    public interface IConfigurationEventPublisher
    {
        Task ConfigurationChangedEventPublish(ConfigurationChangedEvent eventToPublish, CancellationToken cancellationToken = default);
    }
}