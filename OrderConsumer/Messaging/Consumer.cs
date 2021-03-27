using Core.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderConsumer.Messaging
{
    public abstract class Consumer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        protected readonly ILogger _logger;
        protected string RoutingKey;
        protected string QueueName;

        public Consumer(
            ILoggerFactory loggerFactory,
            IConfiguration configuration
        )
        {
            _logger = loggerFactory.CreateLogger<Consumer>();

            try
            {
                var host = configuration.GetSection("RabbitMq:Host").Value;
                var username = configuration.GetSection("RabbitMq:Username").Value;
                var password = configuration.GetSection("RabbitMq:Password").Value;

                var factory = new ConnectionFactory
                {
                    HostName = host,
                    UserName = username,
                    Password = password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"RabbitConsumer init error, {ex}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return Task.CompletedTask;
        }

        private void Register()
        {
            _logger.LogInformation($"RabbitListener register,routeKey:{RoutingKey}");

            _channel.ExchangeDeclare(
                exchange: RabbitMqConstants.DefaultExchange, 
                type: ExchangeType.Topic
            );

            _channel.QueueDeclare(
                queue: QueueName, 
                exclusive: false,
                autoDelete: false
            );

            _channel.QueueBind(
                queue: QueueName,
                exchange: RabbitMqConstants.DefaultExchange,
                routingKey: RoutingKey
            );

            _channel.BasicQos(0, 3, false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);

                if (result)
                {
                    _channel.BasicAck(
                        deliveryTag: ea.DeliveryTag, 
                        multiple: false
                    );
                }
            };

            _channel.BasicConsume(
                queue: QueueName, 
                consumer: consumer
            );
        }

        public abstract bool Process(string message);
    }
}
