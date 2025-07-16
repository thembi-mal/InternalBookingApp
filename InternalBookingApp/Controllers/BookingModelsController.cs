using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InternalBookingApp.Data;
using InternalBookingApp.Models;
using InternalBookingApp.Services;

namespace InternalBookingApp.Controllers
{
    public class BookingModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;

        public BookingModelsController(ApplicationDbContext context, IBookingService bookingService)
        {
            _context = context;
        }

        // GET: BookingModels
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.BookingModels.Include(b => b.ResourceManagement);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: BookingMOdel
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.BookingModels
                .Include(b => b.ResourceManagement)  // Load related resource data
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }


        // GET: BookingModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingModel = await _context.BookingModels
                .Include(b => b.ResourceManagement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookingModel == null)
            {
                return NotFound();
            }

            return View(bookingModel);
        }

        // GET: BookingModels/Create
        //public IActionResult Create()
        //{
        //    ViewData["ResourceId"] = new SelectList(_context.ResourceManagements, "Id", "Name");
        //    return View();
        //}

        public async Task<IActionResult> Create()
        {
            // Create dropdown list of available resources
            ViewBag.ResourceId = new SelectList(
                await _context.ResourceManagements.Where(r => r.IsAvailable).ToListAsync(),
                "Id", "Name"
            );
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,StartTime,EndTime,BookedBy,Purpose")] BookingModel booking)
        {
            // Custom validation
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }

            if (booking.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "Start time cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                //Check for booking conflicts using service
                var isAvailable = await _bookingService.IsResourceAvailable(
                    booking.ResourceId,
                    booking.StartTime,
                    booking.EndTime
                );

                if (!isAvailable)
                {
                    var conflictingBookings = await _bookingService.GetConflictingBookings(
                        booking.ResourceId,
                        booking.StartTime,
                        booking.EndTime
                    );

                    ModelState.AddModelError("",
                        "This resource is already booked during the requested time. " +
                        "Please choose another slot or resource, or adjust your times.");

                    ViewBag.ConflictingBookings = conflictingBookings;
                }
                else
                {
                    try
                    {
                        _context.Add(booking);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Booking created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while creating the booking.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ResourceId = new SelectList(
                await _context.ResourceManagements.Where(r => r.IsAvailable).ToListAsync(),
                "Id", "Name", booking.ResourceId
            );

            return View(booking);
        }


        // POST: BookingModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ResourceId,StartTime,EndTime,BookedBy,Purpose")] BookingModel bookingModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(bookingModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ResourceId"] = new SelectList(_context.ResourceManagements, "Id", "Name", bookingModel.ResourceId);
        //    return View(bookingModel);
        //}

        //// GET: BookingModels/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var bookingModel = await _context.BookingModels.FindAsync(id);
        //    if (bookingModel == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ResourceId"] = new SelectList(_context.ResourceManagements, "Id", "Name", bookingModel.ResourceId);
        //    return View(bookingModel);
        //}

        // POST: BookingModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ResourceId,StartTime,EndTime,BookedBy,Purpose")] BookingModel bookingModel)
        {
            if (id != bookingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingModelExists(bookingModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.ResourceManagements, "Id", "Name", bookingModel.ResourceId);
            return View(bookingModel);
        }

        // GET: BookingModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingModel = await _context.BookingModels
                .Include(b => b.ResourceManagement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookingModel == null)
            {
                return NotFound();
            }

            return View(bookingModel);
        }

        // POST: BookingModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookingModel = await _context.BookingModels.FindAsync(id);
            if (bookingModel != null)
            {
                _context.BookingModels.Remove(bookingModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingModelExists(int id)
        {
            return _context.BookingModels.Any(e => e.Id == id);
        }
    }
}
