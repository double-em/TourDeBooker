using System;

namespace Infrastructure
{
    public static class Constants
    {
        public static class RabbitMq
        {
            public static string Hostname = Environment.GetEnvironmentVariable("RABBIT_HOST");
            public static string Port = Environment.GetEnvironmentVariable("RABBIT_PORT");
            public static string User = Environment.GetEnvironmentVariable("RABBIT_USER");
            public static string Password = Environment.GetEnvironmentVariable("RABBIT_PASSWORD");
        }
    }
}