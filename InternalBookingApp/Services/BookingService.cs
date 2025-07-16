using Microsoft.EntityFrameworkCore;
using InternalBookingApp.Data;
using InternalBookingApp.Models;

namespace InternalBookingApp.Services
{
        public class BookingService : IBookingService
        {
            private readonly ApplicationDbContext _context;

            public BookingService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> IsResourceAvailable(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
            {
                var conflictingBookings = await GetConflictingBookings(resourceId, startTime, endTime, excludeBookingId);
                return !conflictingBookings.Any();
            }

            public async Task<IEnumerable<BookingModel>> GetConflictingBookings(int resourceId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
            {
                var query = _context.BookingModels
                    .Where(b => b.ResourceId == resourceId &&
                               (b.StartTime < endTime && b.EndTime > startTime));

                if (excludeBookingId.HasValue)
                {
                    query = query.Where(b => b.Id != excludeBookingId.Value);
                }

                return await query.ToListAsync();
            }

            public async Task<IEnumerable<BookingModel>> GetUpcomingBookings(int resourceId)
            {
                return await _context.BookingModels
                    .Where(b => b.ResourceId == resourceId && b.StartTime > DateTime.Now)
                    .OrderBy(b => b.StartTime)
                    .Include(b => b.ResourceManagement)
                    .ToListAsync();
            }

            public async Task<IEnumerable<BookingModel>> GetTodaysBookings()
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                return await _context.BookingModels
                    .Where(b => b.StartTime >= today && b.StartTime < tomorrow)
                    .OrderBy(b => b.StartTime)
                    .Include(b => b.ResourceManagement)
                    .ToListAsync();
            }
        }
}
