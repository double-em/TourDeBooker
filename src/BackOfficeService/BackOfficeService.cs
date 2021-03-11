using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace BackOfficeService
{
    public class BackOfficeService : BackgroundService
    {
        private readonly ILogger<BackOfficeService> _logger;

        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        private List<string> messageList = new();

        public BackOfficeService(ILogger<BackOfficeService> logger, IOptions<RabbitMqConfiguration> rabbitMqOptions)
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
            
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: "topic",
                durable: true);

            var args = new Dictionary<string, object> {{"x-dead-letter-exchange", "my-dlx"}};

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);
            
            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _queueName);
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
                
                HandleMessage(ea.BasicProperties, routingKey, booking);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(IBasicProperties properties, string routingKey, BookingModel booking)
        {
            
            
            if (messageList.Count == 3)
            {
                var currentTime = DateTime.Now.ToLocalTime().ToLongTimeString();
                Console.WriteLine($"[{currentTime}] Received '{routingKey}': '{booking}'\n" +
                                  $"[Properties] MessageId: {properties.MessageId}, CorrelationId: {properties.CorrelationId}");

                for (int i = 0; i < messageList.Count; i++)
                {
                    
                }
            }
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