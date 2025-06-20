using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudDevPOE.Data;
using CloudDevPOE.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using CloudDevPOE.Migrations;

namespace CloudDevPOE.Controllers
{
    public class VenuesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAzureBlobStorageService _azureBlobStorageService;


        public VenuesController(AppDbContext context,IAzureBlobStorageService azureBlobStorageService)
        {
            _azureBlobStorageService = azureBlobStorageService;
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();

            venues.ForEach( v =>
            {
                var fileStream =  _azureBlobStorageService.DownloadImageAsync(v.VenueId.ToString()).Result;
                byte[] imageBytes = new byte[fileStream.Length];
                fileStream.ReadExactly(imageBytes);
                v.ImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
            });

            return View(venues);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,Name,Location,Capacity,ImageFile,CreatedAt")] Venue venue)
        {
            if (ModelState.IsValid && venue.ImageFile != null)
            {

                _context.Add(venue);
                await _context.SaveChangesAsync();

                using (var memoryStream = new MemoryStream())
                {
                    await venue.ImageFile.CopyToAsync(memoryStream);
                    await _azureBlobStorageService.UploadImageAsync(memoryStream, venue.VenueId.ToString());
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,Name,Location,Capacity,ImageFile,CreatedAt")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid && venue.ImageFile != null)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        await venue.ImageFile.CopyToAsync(memoryStream);
                        await _azureBlobStorageService.UploadImageAsync(memoryStream, venue.VenueId.ToString());
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venue.Any(e => e.VenueId == id))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
