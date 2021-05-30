using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using AirlineSendAgent.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AirlineSendAgent.App
{
    public class AppHost : IAppHost
    {
        private readonly SendAgentDbContext _dbContext;
        private readonly IWebhookClient _webhookClient;
        public AppHost(SendAgentDbContext dbContext,
            IWebhookClient webhookClient)
        {
            _dbContext = dbContext;
            _webhookClient = webhookClient;
        }

        public void Run()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, 
                exchange: "trigger", 
                routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine("Listening on the message bus...");

            consumer.Received += async (ModuleHandler, ea) =>
            {
                Console.WriteLine("Event is triggered!");
                var body = ea.Body;
                var notificaitonMessage = Encoding.UTF8.GetString(body.ToArray());
                var message = JsonSerializer.Deserialize<NotificationMessageDto>(notificaitonMessage);

                var webhookToSend = new FlightDetailChangePayloadDto()
                {
                    WebhookType = message.WebhookType,
                    WebhookUri = string.Empty,
                    Secret = string.Empty,
                    Publisher = string.Empty,
                    OldPrice = message.OldPrice,
                    NewPrice = message.NewPrice,
                    FlightCode = message.FlightCode
                };

                foreach(var whs in _dbContext.WebhookSubscriptions
                    .Where(s => s.WebhookType.Equals(message.WebhookType)))
                {
                    webhookToSend.WebhookUri = whs.WebhookUri;
                    webhookToSend.Secret = whs.Secret;
                    webhookToSend.Publisher = whs.WebhookPublisher;

                    await _webhookClient.SendWebhookNotification(webhookToSend);
                }
            };

            channel.BasicConsume(
                queue: queueName,
                autoAck: true, 
                consumer: consumer);

            Console.ReadLine();
        }
    }
}
