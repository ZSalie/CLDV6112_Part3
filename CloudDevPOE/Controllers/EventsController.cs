using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudDevPOE.Data;
using CloudDevPOE.Models;

namespace CloudDevPOE.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .ToListAsync();

            // Ensure correct date-time formatting in the model before passing to view
            events.ForEach(e =>
            {
                e.StartDate = e.StartDate.ToLocalTime();
                e.EndDate = e.EndDate.ToLocalTime();
            });


            ViewData["FilterEventType"] = new SelectList(_context.EventType, "EventTypeId", "Name");

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(x => x.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            // Ensure StartDate and EndDate include time components
            @event.StartDate = @event.StartDate.ToLocalTime();
            @event.EndDate = @event.EndDate.ToLocalTime();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name");
            ViewData["EventType"] = new SelectList(_context.EventType, "EventTypeId", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Name,Description,StartDate,EndDate,VenueId,EventTypeId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.StartDate = @event.StartDate.ToLocalTime();
                @event.EndDate = @event.EndDate.ToLocalTime();

                bool hasConflict = await _context.Event.AnyAsync(e => e.VenueId == @event.VenueId &&
                    e.StartDate.Date == @event.StartDate.Date);

                if (hasConflict)
                {
                    ModelState.AddModelError("", "The venue is already booked for an event on the selected date. Please choose a different day.");
                }
                else
                {
                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", @event.VenueId);
            ViewData["EventType"] = new SelectList(_context.EventType, "EventTypeId", "Name", @event.EventType);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.StartDate = @event.StartDate.ToLocalTime();
            @event.EndDate = @event.EndDate.ToLocalTime();

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", @event.VenueId);
            ViewData["EventType"] = new SelectList(_context.EventType, "EventTypeId", "Name", @event.EventTypeId);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,Description,StartDate,EndDate,VenueId,EventTypeId")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @event.StartDate = @event.StartDate.ToLocalTime();
                    @event.EndDate = @event.EndDate.ToLocalTime();

                    bool hasConflict = await _context.Event.AnyAsync(e => e.VenueId == @event.VenueId &&
                        e.EventId != @event.EventId &&
                        e.StartDate.Date == @event.StartDate.Date);

                    if (hasConflict)
                    {
                        ModelState.AddModelError("", "The venue is already booked for an event on the selected date. Please choose a different day.");
                    }
                    else
                    {
                        _context.Update(@event);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", @event.VenueId);
            ViewData["EventType"] = new SelectList(_context.EventType, "EventTypeId", "Name", @event.EventTypeId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(x => x.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                bool hasBookings = await _context.Booking.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    ViewBag.Message = "You cannot delete this as there are bookings for this event";
                    return View(@event);
                }
                else
                {
                    _context.Event.Remove(@event);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ?? GET: Events/Filter
        public async Task<IActionResult> Filter(string? filterName, int? filterEventType, DateTime? filterStartDate, DateTime? filterEndDate)
        {
            var query = _context.Event
                .Include(e => e.EventType)
                .Include(e => e.Venue)
                .Where(e => e.Venue.IsAvailable);

            if (!string.IsNullOrWhiteSpace(filterName))
                query = query.Where(e => e.Name.Contains(filterName));

            if (filterEventType.HasValue)
                query = query.Where(e => e.EventTypeId == filterEventType);

            if (filterStartDate.HasValue && filterEndDate.HasValue)
                query = query.Where(e => e.StartDate >= filterStartDate && e.EndDate <= filterEndDate);

            var filteredEvents = await query.ToListAsync();


            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name");
            ViewData["FilterEventType"] = new SelectList(_context.EventType, "EventTypeId", "Name");

            return View("Index", filteredEvents);
        }

        public async Task<IActionResult> ClearFilter()
        {
            // Clear the filter parameters and redirect to Index
            return await Task.FromResult(RedirectToAction(nameof(Index)));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
        //Search Function
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View("Index", await _context.Event.Include(b => b.EventType).Include(e => e.Venue).ToListAsync());
            }

            var results = await _context.Event
                .Include(b => b.EventType)
                .Include(e => e.Venue)
                .Where(b => b.Name.Contains(searchQuery))
                .ToListAsync();

            return View("Index", results);
        }
    }
}
