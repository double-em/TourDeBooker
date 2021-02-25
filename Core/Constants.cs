namespace Core
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
    }
}