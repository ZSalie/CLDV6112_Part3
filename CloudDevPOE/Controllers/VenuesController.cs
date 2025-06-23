using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudDevPOE.Data;
using CloudDevPOE.Models;
using Microsoft.AspNetCore.Http;

namespace CloudDevPOE.Controllers
{
    public class VenuesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IImageRepository _imageRepository;

        public VenuesController(AppDbContext context, IImageRepository imageRepository)
        {
            _context = context;
            _imageRepository = imageRepository;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();

            foreach (var v in venues)
            {
                var fileStream = await _imageRepository.DownloadImageAsync(v.VenueId.ToString());
                if (fileStream != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(ms);
                        v.ImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                    }
                }
            }

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
        public async Task<IActionResult> Create([Bind("VenueId,Name,Location,Capacity,ImageFile,CreatedAt,IsAvailable")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile == null)
                {
                    ModelState.AddModelError("ImageFile", "Image upload is required.");
                    return View(venue);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();

                using (var memoryStream = new MemoryStream())
                {
                    await venue.ImageFile.CopyToAsync(memoryStream);
                    await _imageRepository.UploadImageAsync(memoryStream, venue.VenueId.ToString());
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,Name,Location,Capacity,ImageFile,CreatedAt,IsAvailable")] Venue venue)
        {
            if (id != venue.VenueId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();

                    if (venue.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await venue.ImageFile.CopyToAsync(memoryStream);
                            await _imageRepository.UploadImageAsync(memoryStream, venue.VenueId.ToString());
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venue.Any(e => e.VenueId == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
                return NotFound();

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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
