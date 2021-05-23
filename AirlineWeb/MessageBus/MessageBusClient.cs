using AirlineWeb.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace AirlineWeb.MessageBus
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly ILogger<MessageBusClient> _logger;
        public MessageBusClient(ILogger<MessageBusClient> logger)
        {
            _logger = logger;
        }

        public void SendMessage(NotificationMessageDto notificationMessageDto)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            var message = JsonConvert.SerializeObject(notificationMessageDto);
            var body = Encoding.UTF8.GetBytes(message);


            channel.BasicPublish(exchange: "trigger", 
                routingKey: "",
                basicProperties: null, 
                body: body);


            _logger.LogInformation($"Message successfully published, id: {notificationMessageDto.Id}");
        }
    }
}
