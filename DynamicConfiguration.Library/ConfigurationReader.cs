using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Library.Interfaces;
using DynamicConfiguration.Library.Models;
using DynamicConfiguration.Library.Storage;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DynamicConfiguration.Library
{
    public class ConfigurationReader : IConfigurationReader, IDisposable
    {
        private const string ExchangeName = "config-updates";
        
        private readonly MongoConfigurationStore _store;

        private readonly Timer _timer;

        private Dictionary<string, ConfigurationItem> _cache;
        
        private readonly IConnection _rabbitConnection;
        
        private readonly IChannel _rabbitChannel;
        
        private string ApplicationName { get; }
        
        public ConfigurationReader(string applicationName, string connectionString, int refreshIntervalMs)
        {
            var connections = connectionString.Split('|');
            _store = new MongoConfigurationStore(connections[0]);
            _cache = new Dictionary<string, ConfigurationItem>();
            ApplicationName = applicationName;

            LoadConfigs()
                .GetAwaiter()
                .GetResult();

            _timer = new Timer(_ => LoadConfigs(),
                null,
                refreshIntervalMs,
                refreshIntervalMs);
            if (connections.Length > 1)
            {
                (_rabbitConnection, _rabbitChannel) =
                    SetupRabbitMq(connections[1], ExchangeName)
                        .GetAwaiter()
                        .GetResult();
            }
        }

        
        public T GetValue<T>(string key)
        {
            if (!_cache.TryGetValue(key, out var item))
            {
                return default;
            }
            
            if (typeof(T) == typeof(bool))
            {
                return item.Value switch
                {
                    "1" => (T)Convert.ChangeType(true, typeof(T)),
                    "0" => (T)Convert.ChangeType(false, typeof(T)),
                    _ => (T)Convert.ChangeType(item.Value, typeof(T))
                };
            }
                
            return (T)Convert.ChangeType(item.Value, typeof(T));
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _rabbitConnection?.Dispose();
            _rabbitChannel?.Dispose();
        }

        private async Task LoadConfigs()
        {
            try
            {
                var configs = await _store.GetActiveConfigs(ApplicationName);
                _cache = configs.ToDictionary(c => c.Name, c => c);
            }
            catch
            {
                // Logging or fallback
            }
        }
        
        private async Task<(IConnection,IChannel)> SetupRabbitMq(string rabbitConnString,string exchangeName)
        {
            var factory = new ConnectionFactory {
                HostName = rabbitConnString,
                UserName = "guest",
                Password = "guest"
            };
            
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true);
            
            var queueName = (await channel.QueueDeclareAsync()).QueueName;
            
            await channel.QueueBindAsync(queueName, exchangeName, routingKey: ApplicationName);
            
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    await LoadConfigs();
                    
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            return (connection, channel);
        }
    }
}