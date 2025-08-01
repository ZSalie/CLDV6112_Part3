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
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await  _context.Booking
                .Include(b => b.Event)
                .ToListAsync();

            ViewData["FilterEventType"] = new SelectList(_context.EventType, "EventTypeId", "Name");

            return View( bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CustomerName,CustomerContact,EventId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,CustomerName,CustomerContact,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
        //Search Function
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View("Index", await _context.Booking.Include(b => b.Event).ThenInclude(e => e.Venue).ToListAsync());
            }

            var results = await _context.Booking
                .Include(b => b.Event).ThenInclude(e => e.Venue)
                .Where(b => b.BookingId.ToString().Contains(searchQuery) ||
                       b.Event.Name.Contains(searchQuery))
                .ToListAsync();

            return View("Index", results);
        }
        public async Task<IActionResult> Filter(string? filterName, int? filterEventType, DateTime? filterStartDate, DateTime? filterEndDate, bool? filterAvailability)
        {
            var query = _context.Booking
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .AsQueryable();
                
               
            if (!string.IsNullOrWhiteSpace(filterName))
                query = query.Where(b => b.CustomerName!.Contains(filterName));

            if (filterEventType.HasValue)
                query = query.Where(b => b.Event!.EventTypeId == filterEventType);

            if (filterStartDate.HasValue && filterEndDate.HasValue)
                query = query.Where(b => b.BookingDate >= filterStartDate && b.BookingDate <= filterEndDate);



            //if (filterAvailability.HasValue)
            //    query = query.Select(
            //            b => 
            //            new
            //            {
            //                Booking = b,
            //                Capacity = b.Event!.Venue!.Capacity,
            //                Count = _context.Set<Booking>().Include(ib => ib.EventId == b.EventId).Count()
            //            } 
            //        )
            //        .Where(x => x.Count < x.Capacity)
            //        .Select(x => x.Booking);

            if (filterAvailability.HasValue)
                query = query.Select(b => new
                {
                    BookingCount = _context.Booking.Count(x => x.EventId == b.EventId),
                    Capacity = b.Event!.Venue!.Capacity,
                    Booking = b
                })
                    .Where(x => x.BookingCount < x.Capacity)
                    .Select(x => x.Booking)
                    .AsQueryable<Booking>();
                    


            var filteredBookings = await query.ToListAsync();

            //if (filterAvailability.HasValue)
            //{
            //    filteredBookings = filteredBookings.ForEach(async b =>
            //    {
            //        count = await _context.Booking.CountAsync(ib => ib.EventId == b.EventId),

            //    })
            //}

            // filteredEvents.ForEach(e =>
            // {
            //     e.StartDate = e.StartDate.ToLocalTime();
            //     e.EndDate = e.EndDate.ToLocalTime();
            // });

            //ViewData["BookingId"] = new SelectList(_context.Booking, "BookingId", "Name");
            ViewData["FilterEventType"] = new SelectList(_context.EventType, "EventTypeId", "Name");

            return View("Index", filteredBookings);
        }

        public async Task<IActionResult> ClearFilter()
        {
            // Clear the filter parameters and redirect to Index
            return await Task.FromResult(RedirectToAction(nameof(Index)));
        }

    }

}
