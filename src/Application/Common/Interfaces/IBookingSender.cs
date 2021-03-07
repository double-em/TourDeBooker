using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IBookingSender
    {
        void SendBooking(string queue, BookingModel booking);
    }
}