using System;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Services.Options;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class BookingSender : IMessageSender
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _rabbitMqUser;
        private readonly string _rabbitMqPass;
        private IConnection _connection;

        public BookingSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _rabbitMqUser = rabbitMqOptions.Value.Username;
            _rabbitMqPass = rabbitMqOptions.Value.Password;
            
            CreateConnection();
            
        }

        public void SendBooking(string queue, BookingModel booking)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.ExchangeDeclare(
                        exchange: Constants.Exchange.TourBooking,
                        type: "topic");

                    var json = JsonConvert.SerializeObject(booking);
                    var body = Encoding.UTF8.GetBytes(json);
                    
                    channel.BasicPublish(
                        exchange: Constants.Exchange.TourBooking,
                        routingKey: queue,
                        basicProperties: null,
                        body: body);
                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _rabbitMqUser,
                    Password = _rabbitMqPass
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not create connection on '{_hostname}: {e.Message}'");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}