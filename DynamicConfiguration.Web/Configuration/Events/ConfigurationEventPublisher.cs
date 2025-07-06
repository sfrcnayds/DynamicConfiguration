using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Common.Events;
using DynamicConfiguration.Web.Configuration.Interfaces;
using DynamicConfiguration.Web.Configuration.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DynamicConfiguration.Web.Configuration.Events
{
    public class ConfigurationEventPublisher : IConfigurationEventPublisher
    {
        private const string ExchangeName = "config-updates";
        private readonly IOptions<RabbitMqOptions> _rabbitMqOptions;
        
        public ConfigurationEventPublisher(IOptions<RabbitMqOptions> rabbitMqOptions)
        {
            _rabbitMqOptions = rabbitMqOptions;
        }

        public async Task ConfigurationChangedEventPublish(ConfigurationChangedEvent eventToPublish, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory {
                HostName = _rabbitMqOptions.Value.HostName,
                UserName = _rabbitMqOptions.Value.User,
                Password = _rabbitMqOptions.Value.Pass
            };
            
            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            
            await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
            
            var evt = new ConfigurationChangedEvent{
                ApplicationName = eventToPublish.ApplicationName,
                Action = eventToPublish.Action,
                Timestamp = DateTime.UtcNow
            };
            
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));
            await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: eventToPublish.ApplicationName, body: body, cancellationToken: cancellationToken);
        }
        
    }
}