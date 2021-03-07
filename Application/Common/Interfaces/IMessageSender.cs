using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IMessageSender
    {
        void SendBooking(string queue, BookingModel booking);
    }
}