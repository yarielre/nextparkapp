using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inside.Domain.Entities;
using Inside.Web.Data;
using Microsoft.AspNetCore.Authorization;

namespace Inside.Web.Controllers
{
    [Authorize]
    public class ParkingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parkings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Parkings.Include(p => p.ParkingCategory).Include(p => p.ParkingEvent).Include(p => p.ParkingType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Parkings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parkings
                .Include(p => p.ParkingCategory)
                .Include(p => p.ParkingEvent)
                .Include(p => p.ParkingType)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // GET: Parkings/Create
        public IActionResult Create()
        {
            ViewData["ParkingCategoryId"] = new SelectList(_context.ParkingCategories, "Id", "Id");
            ViewData["ParkingEventId"] = new SelectList(_context.Events, "Id", "Id");
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Id");
            return View();
        }

        // POST: Parkings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParkingCategoryId,ParkingEventId,ParkingTypeId,ImageUrl,IsRented,Latitude,Longitude,UserId")] Parking parking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParkingCategoryId"] = new SelectList(_context.ParkingCategories, "Id", "Id", parking.ParkingCategoryId);
            ViewData["ParkingEventId"] = new SelectList(_context.Events, "Id", "Id", parking.ParkingEventId);
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Id", parking.ParkingTypeId);
            return View(parking);
        }

        // GET: Parkings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parkings.SingleOrDefaultAsync(m => m.Id == id);
            if (parking == null)
            {
                return NotFound();
            }
            ViewData["ParkingCategoryId"] = new SelectList(_context.ParkingCategories, "Id", "Id", parking.ParkingCategoryId);
            ViewData["ParkingEventId"] = new SelectList(_context.Events, "Id", "Id", parking.ParkingEventId);
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Id", parking.ParkingTypeId);
            return View(parking);
        }

        // POST: Parkings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParkingCategoryId,ParkingEventId,ParkingTypeId,ImageUrl,IsRented,Latitude,Longitude,UserId")] Parking parking)
        {
            if (id != parking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingExists(parking.Id))
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
            ViewData["ParkingCategoryId"] = new SelectList(_context.ParkingCategories, "Id", "Id", parking.ParkingCategoryId);
            ViewData["ParkingEventId"] = new SelectList(_context.Events, "Id", "Id", parking.ParkingEventId);
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Id", parking.ParkingTypeId);
            return View(parking);
        }

        // GET: Parkings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parking = await _context.Parkings
                .Include(p => p.ParkingCategory)
                .Include(p => p.ParkingEvent)
                .Include(p => p.ParkingType)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parking == null)
            {
                return NotFound();
            }

            return View(parking);
        }

        // POST: Parkings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parking = await _context.Parkings.SingleOrDefaultAsync(m => m.Id == id);
            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingExists(int id)
        {
            return _context.Parkings.Any(e => e.Id == id);
        }
    }
}
