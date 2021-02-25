namespace WebApp.Models
{
    public enum BookingType
    {
        Book,
        Cancel
    }
    public class BookingModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Tour { get; set; }
        
        public BookingType ActionType { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Email}, {Tour}, {ActionType.ToString().ToLower()}";
        }
    }
}