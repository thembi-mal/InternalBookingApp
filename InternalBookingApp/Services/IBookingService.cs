using InternalBookingApp.Models;


namespace InternalBookingApp.Services
{
    public interface IBookingService
    {
        Task<bool> IsResourceAvailable(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        Task<IEnumerable<BookingModel>> GetConflictingBookings(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        Task<IEnumerable<BookingModel>> GetUpcomingBookings(int resourceId);
        Task<IEnumerable<BookingModel>> GetTodaysBookings();
    }
}
