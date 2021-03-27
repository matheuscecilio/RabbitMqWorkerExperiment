using Core.Constants;
using Core.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderConsumer.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProducer.BackgroundServices
{
    public class OrderProducerService : BackgroundService
    {
        private readonly IProducer _producer;
        private readonly ILogger _logger;

        public OrderProducerService(
            IProducer producer, 
            ILoggerFactory loggerFactory
        )
        {
            _producer = producer;
            _logger = loggerFactory.CreateLogger<OrderProducerService>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var order = new Order
                {
                    Description = $"Order {random.Next()}",
                    Price = random.NextDouble()
                };

                _producer.PublishMessage(
                    RabbitMqConstants.DefaultOrderRoutingKey,
                    order
                );

                _logger.LogInformation($"Order sent. RoutingKey: {RabbitMqConstants.DefaultOrderRoutingKey}");

                var randomSeconds = random.Next(1, 3) * 1000;
                Thread.Sleep(randomSeconds);
            }

            return Task.CompletedTask;
        }
    }
}
