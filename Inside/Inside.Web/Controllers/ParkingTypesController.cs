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
    public class ParkingTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParkingTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ParkingTypes.ToListAsync());
        }

        // GET: ParkingTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingType = await _context.ParkingTypes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parkingType == null)
            {
                return NotFound();
            }

            return View(parkingType);
        }

        // GET: ParkingTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ParkingTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type")] ParkingType parkingType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkingType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parkingType);
        }

        // GET: ParkingTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingType = await _context.ParkingTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (parkingType == null)
            {
                return NotFound();
            }
            return View(parkingType);
        }

        // POST: ParkingTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type")] ParkingType parkingType)
        {
            if (id != parkingType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkingType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingTypeExists(parkingType.Id))
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
            return View(parkingType);
        }

        // GET: ParkingTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingType = await _context.ParkingTypes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parkingType == null)
            {
                return NotFound();
            }

            return View(parkingType);
        }

        // POST: ParkingTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkingType = await _context.ParkingTypes.SingleOrDefaultAsync(m => m.Id == id);
            _context.ParkingTypes.Remove(parkingType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingTypeExists(int id)
        {
            return _context.ParkingTypes.Any(e => e.Id == id);
        }
    }
}
