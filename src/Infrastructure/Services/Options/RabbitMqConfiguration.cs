namespace Infrastructure.Services.Options
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public bool Enabled { get; set; }
    }
}