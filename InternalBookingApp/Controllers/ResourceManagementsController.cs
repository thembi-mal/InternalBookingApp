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
using Elfie.Serialization;

namespace InternalBookingApp.Controllers
{
    public class ResourceManagementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingService _bookingService;

        public ResourceManagementsController(ApplicationDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;

        }

        // GET: ResourceManagements

        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.ResourceManagements.ToListAsync());
        //}

        public async Task<IActionResult> Index()
        {
            try
            {
                var resources = await _context.ResourceManagements.ToListAsync();
                return View(resources);
            }
            catch (Exception ex)
            {
                // Log error (in real app, use ILogger)
                TempData["Error"] = "Unable to load resources.";
                return View(new List<ResourceManagement>());
            }
        }

        // GET: ResourceManagements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceManagement = await _context.ResourceManagements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceManagement == null)
            {
                return NotFound();
            }

            return View(resourceManagement);
        }

        // GET: ResourceManagements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResourceManagements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Location,Capacity,IsAvailable")] ResourceManagement resourceManagement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resourceManagement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resourceManagement);
        }

        // GET: ResourceManagements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceManagement = await _context.ResourceManagements.FindAsync(id);
            if (resourceManagement == null)
            {
                return NotFound();
            }
            return View(resourceManagement);
        }

        // POST: ResourceManagements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Location,Capacity,IsAvailable")] ResourceManagement resourceManagement)
        {
            if (id != resourceManagement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resourceManagement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceManagementExists(resourceManagement.Id))
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
            return View(resourceManagement);
        }

        // GET: ResourceManagements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceManagement = await _context.ResourceManagements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceManagement == null)
            {
                return NotFound();
            }

            return View(resourceManagement);
        }

        // POST: ResourceManagements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resourceManagement = await _context.ResourceManagements.FindAsync(id);
            if (resourceManagement != null)
            {
                _context.ResourceManagements.Remove(resourceManagement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceManagementExists(int id)
        {
            return _context.ResourceManagements.Any(e => e.Id == id);
        }
    }
}
