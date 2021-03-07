using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IBookingSender
    {
        void SendBooking(BookingModel booking);
    }
}