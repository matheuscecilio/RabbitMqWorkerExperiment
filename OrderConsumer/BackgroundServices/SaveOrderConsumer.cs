using Core.Constants;
using Core.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderConsumer.Messaging;
using System;
using System.Threading;

namespace OrderConsumer.BackgroundServices
{
    public class SaveOrderConsumer : Consumer
    {
        public SaveOrderConsumer(
            ILoggerFactory loggerFactory,
            IConfiguration configuration
        ) : base(loggerFactory, configuration) 
        {
            RoutingKey = RabbitMqConstants.DefaultOrderRoutingKey;
            QueueName = RabbitMqConstants.DefaultOrderQueue;
        }

        public override bool Process(string message)
        {
            try
            {
                var order = JsonConvert.DeserializeObject<Order>(message);

                _logger.LogInformation($"Message received: {order}");

                // Simulating order persistence
                order.Id = Guid.NewGuid();
                var random = new Random();
                var randomSeconds = random.Next(1, 5) * 1000;
                Thread.Sleep(randomSeconds);

                _logger.LogInformation($"Order seved succesfully: {order.Id}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during order processing: {ex}");
                return false;
            }
        }
    }
}
