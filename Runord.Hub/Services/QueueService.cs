using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Runord.Hub.Configs;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.DTOs.Task;
using System.Text;
using System.Text.Json;

namespace Runord.Hub.Services
{
    public class QueueService : IQueueService, IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;
        private const string TaskQueueName = "runord_tasks";
        private const string ResultQueueName = "runord_results";
        private bool _initialized;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public QueueService(IOptions<RabbitMqSettings> options)
        {
            _factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                VirtualHost = options.Value.VirtualHost
            };
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
        {
            if (_initialized) return;
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_initialized) return;
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync(TaskQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync(ResultQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                _initialized = true;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> PublishTaskAsync(TaskQueueMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureInitializedAsync(cancellationToken);
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                var properties = new BasicProperties { Persistent = true, Priority = (byte)message.Priority };
                await _channel!.BasicPublishAsync(string.Empty, TaskQueueName, false, properties, body, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SubscribeTaskQueue(Func<TaskQueueMessage, CancellationToken, Task> onMessage)
        {
            _ = Task.Run(async () =>
            {
                await EnsureInitializedAsync();
                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<TaskQueueMessage>(json);
                    if (message != null)
                        await onMessage(message, CancellationToken.None);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                };
                await _channel!.BasicConsumeAsync(TaskQueueName, false, consumer);
            });
        }

        public void SubscribeResultQueue(Func<TaskResultMessage, CancellationToken, Task> onMessage)
        {
            _ = Task.Run(async () =>
            {
                await EnsureInitializedAsync();
                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<TaskResultMessage>(json);
                    if (message != null)
                        await onMessage(message, CancellationToken.None);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                };
                await _channel!.BasicConsumeAsync(ResultQueueName, false, consumer);
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            _lock.Dispose();
        }
    }
}