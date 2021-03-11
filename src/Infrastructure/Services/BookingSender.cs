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
    public class BookingSender : IBookingSender
    {
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _rabbitMqUser;
        private readonly string _rabbitMqPass;
        private IConnection _connection;

        public BookingSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _exchangeName = rabbitMqOptions.Value.Exchange;
            _rabbitMqUser = rabbitMqOptions.Value.Username;
            _rabbitMqPass = rabbitMqOptions.Value.Password;
            
            CreateConnection();
            
        }

        public void SendBooking(BookingModel booking)
        {
            if (!ConnectionExists()) return;

            using var channel = _connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: "topic",
                durable: true);

            for (var i = 0; i < 3; i++)
            {
                var json = JsonConvert.SerializeObject(booking);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                    
                properties.DeliveryMode = 2;
                properties.MessageId = i.ToString();
                
                channel.ConfirmSelect();
                    
                channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: $"{_exchangeName}.{booking.ActionType.ToString().ToLower()}",
                    basicProperties: properties,
                    body: body);
                    
                channel.WaitForConfirmsOrDie();
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