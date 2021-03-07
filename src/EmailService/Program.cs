using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = Core.Constants;

namespace EmailService 
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange: Constants.Exchange.TourBooking,
                type: "topic",
                durable: true);
            
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
            
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        static IConnection CreateConnection()
        {
            IConnection connection = null;

            while (connection == null)
            {
                connection = CreateConnectionByFactory();
                Thread.Sleep(1000);
            }

            return connection;
        }

        static IConnection CreateConnectionByFactory()
        {
            var hostname = Constants.RabbitMq.Hostname;
            
            try
            {
                var factory = new ConnectionFactory {HostName = hostname};
                return factory.CreateConnection();
            }
            catch (Exception)
            { 
                Console.WriteLine($"Couldn't connect to the Message System on host: '{hostname}'");
                return null;
            }
        }
    }
}