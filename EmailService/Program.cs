using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = Core.Constants;

namespace EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange: Constants.Exchange.TourBooking,
                type: "topic");
            
            var queueName = channel.QueueDeclare().QueueName;
            
            channel.QueueBind(queue: queueName,
                exchange: Constants.Exchange.TourBooking,
                routingKey: Constants.Channels.Tour.Booked);

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                    routingKey,
                    message);
            };
            
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}