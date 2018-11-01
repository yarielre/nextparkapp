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
    public class ParkingCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParkingCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ParkingCategories.ToListAsync());
        }

        // GET: ParkingCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingCategory = await _context.ParkingCategories
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parkingCategory == null)
            {
                return NotFound();
            }

            return View(parkingCategory);
        }

        // GET: ParkingCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ParkingCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category,MonthPrice,HourPrice,Id")] ParkingCategory parkingCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parkingCategory);
        }

        // GET: ParkingCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingCategory = await _context.ParkingCategories.SingleOrDefaultAsync(m => m.Id == id);
            if (parkingCategory == null)
            {
                return NotFound();
            }
            return View(parkingCategory);
        }

        // POST: ParkingCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Category,MonthPrice,HourPrice,Id")] ParkingCategory parkingCategory)
        {
            if (id != parkingCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkingCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingCategoryExists(parkingCategory.Id))
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
            return View(parkingCategory);
        }

        // GET: ParkingCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingCategory = await _context.ParkingCategories
                .SingleOrDefaultAsync(m => m.Id == id);
            if (parkingCategory == null)
            {
                return NotFound();
            }

            return View(parkingCategory);
        }

        // POST: ParkingCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkingCategory = await _context.ParkingCategories.SingleOrDefaultAsync(m => m.Id == id);
            _context.ParkingCategories.Remove(parkingCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingCategoryExists(int id)
        {
            return _context.ParkingCategories.Any(e => e.Id == id);
        }
    }
}
