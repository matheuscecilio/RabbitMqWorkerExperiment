using Core.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderConsumer.Interfaces;
using RabbitMQ.Client;
using System;
using System.Text;

namespace OrderProducer.Messaging
{
    public class Producer : IProducer
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;

        public Producer(
            ILoggerFactory loggerFactory,
            IConfiguration configuration
        )
        {
            _logger = loggerFactory.CreateLogger<Producer>();
            try
            {
                var host = configuration.GetSection("RabbitMq:Host").Value;
                var username = configuration.GetSection("RabbitMq:Username").Value;
                var password = configuration.GetSection("RabbitMq:Password").Value;

                var factory = new ConnectionFactory()
                {
                    HostName = host,
                    UserName = username,
                    Password = password
                };

                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
        }

        public void PublishMessage(string routingKey, object message)
        {
            _logger.LogInformation($"PublishMessage, routingKey: {routingKey}");

            _channel.ExchangeDeclare(
                exchange: RabbitMqConstants.DefaultExchange,
                type: ExchangeType.Topic
            );

            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);

            _channel.BasicPublish(
                exchange: RabbitMqConstants.DefaultExchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
        }
    }
}
