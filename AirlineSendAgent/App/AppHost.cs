using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace AirlineSendAgent.App
{
    public class AppHost : IAppHost
    {
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
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}
