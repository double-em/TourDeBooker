﻿using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using Constants = Core.Constants;

namespace WebApp.Service
{
    public static class Publisher
    {
        public static void PublishMessage(string routingKey, string message)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange: Constants.Exchange.TourBooking,
                type: "topic");

            var body = Encoding.UTF8.GetBytes(message);
            
            channel.BasicPublish(
                exchange: Constants.Exchange.TourBooking,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
            
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }
}