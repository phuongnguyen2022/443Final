using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastFoodWebApplication.Data;
using FastFoodWebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace FastFoodWebApplication.Controllers
{
    public class DishTypesController : Controller
    {
        private readonly FastFoodWebApplicationContext _context;

        public DishTypesController(FastFoodWebApplicationContext context)
        {
            _context = context;
        }

        // GET: DishTypes
        public async Task<IActionResult> Index()
        {

              return _context.DishType != null ? 
                          View(await _context.DishType.ToListAsync()) :
                          Problem("Entity set 'FastFoodWebApplicationContext.DishType'  is null.");
        }

        // GET: DishTypes/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DishType == null)
            {
                return NotFound();
            }

            var dishType = await _context.DishType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dishType == null)
            {
                return NotFound();
            }

            return View(dishType);
        }

        // GET: DishTypes/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DishTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Name")] DishType dishType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dishType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dishType);
        }

        // GET: DishTypes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DishType == null)
            {
                return NotFound();
            }

            var dishType = await _context.DishType.FindAsync(id);
            if (dishType == null)
            {
                return NotFound();
            }
            return View(dishType);
        }

        // POST: DishTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DishType dishType)
        {
            if (id != dishType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dishType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishTypeExists(dishType.Id))
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
            return View(dishType);
        }

        // GET: DishTypes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DishType == null)
            {
                return NotFound();
            }

            var dishType = await _context.DishType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dishType == null)
            {
                return NotFound();
            }

            return View(dishType);
        }

        // POST: DishTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DishType == null)
            {
                return Problem("Entity set 'FastFoodWebApplicationContext.DishType'  is null.");
            }
            var dishType = await _context.DishType.FindAsync(id);
            if (dishType != null)
            {
                _context.DishType.Remove(dishType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishTypeExists(int id)
        {
          return (_context.DishType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
