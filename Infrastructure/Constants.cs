using System;

namespace Infrastructure
{
    public static class Constants
    {
        public static class Exchange
        {
            public static string TourBooking = "tour_booking";
        }
        
        public static class Channels
        {
            public static class Tour
            {
                public static string TourSubChannels = "tour.*";
                public static string Booked = "tour.booked";
                public static string Cancelled = "tour.cancelled";
            }
        }
        
        public static class RabbitMq
        {
            public static string Hostname = Environment.GetEnvironmentVariable("RABBIT_HOST");
            public static string Port = Environment.GetEnvironmentVariable("RABBIT_PORT");
            public static string User = Environment.GetEnvironmentVariable("RABBIT_USER");
            public static string Password = Environment.GetEnvironmentVariable("RABBIT_PASSWORD");
        }
    }
}