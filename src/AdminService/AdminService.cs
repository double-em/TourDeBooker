using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AdminService
{
    public class AdminService : BackgroundService
    {
        private readonly ILogger<AdminService> _logger;

        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public AdminService(ILogger<AdminService> logger, IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _logger = logger;
            
            _hostname = rabbitMqOptions.Value.Hostname;
            _exchangeName = rabbitMqOptions.Value.Exchange;
            _queueName = $"{_exchangeName}.{rabbitMqOptions.Value.Queue}";
            _username = rabbitMqOptions.Value.Username;
            _password = rabbitMqOptions.Value.Password;
            
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMq_ConnectionShutdown;
            _channel = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var booking = JsonConvert.DeserializeObject<BookingModel>(message);
                var routingKey = ea.RoutingKey;
                
                HandleMessage(routingKey, booking);
                
                _channel.BasicNack(ea.DeliveryTag, false, false);
            };

            _channel.BasicConsume(
                queue: "dead-letter-queue",
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(string routingKey, BookingModel booking)
        {
            var currentTime = DateTime.Now.ToLocalTime().ToLongTimeString();
            Console.WriteLine($"[{currentTime}] Received '{routingKey}': '{booking}'");

        }

        private void RabbitMq_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}