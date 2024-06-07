using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastFoodWebApplication.Data;
using FastFoodWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace FastFoodWebApplication.Controllers
{
    [Authorize(Roles = "admin")]
    public class DishesController : Controller
    {
        private readonly FastFoodWebApplicationContext _context;
        private readonly String _webRoot;
        public DishesController(FastFoodWebApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _webRoot = env.WebRootPath;

        }

        // GET: Dishes
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? dishTypeid, string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            IQueryable<Dish> dishQuery = _context.Dish.Include(x => x.DishType);

            var dishes = from d in dishQuery
                         select d;
            if (!String.IsNullOrEmpty(searchString))
            {
                dishes = dishes.Where(d => d.DishType.Name.Contains(searchString)
                                       || d.Name.Contains(searchString));
            }
            if (dishTypeid != null)
            {
                dishes = dishes.Where(x => x.DishTypeId == dishTypeid);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    dishes = dishes.OrderByDescending(s => s.Name);
                    break;
                case "Category":
                    dishes = dishes.OrderBy(s => s.DishType.Name);
                    break;
                case "category_desc":
                    dishes = dishes.OrderByDescending(s => s.DishType.Name);
                    break;
                case "price_desc":
                    dishes = dishes.OrderByDescending(s => s.DishPrice);
                    break;
                case "Price":
                    dishes = dishes.OrderBy(s => s.DishPrice);
                    break;
                default:
                    dishes = dishes.OrderBy(s => s.Name);
                    break;
            }
            var pageSize = 3;
            var model = await PaginatedList<Dish>.CreateAsync(dishes, pageNumber ?? 1, pageSize);
            return View(model);
            //var fastFoodWebApplicationContext = _context.Dish.Include(d => d.DishType);
            //return View(await fastFoodWebApplicationContext.ToListAsync());
        }

        // GET: Dishes/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Dish == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .Include(d => d.DishType)
                .FirstOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["DishTypeId"] = new SelectList(_context.Set<DishType>(), nameof(DishType.Id), nameof(DishType.Name));
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("DishId,Name,DishSize,Description,DishStatus,DishTypeId,DishPrice,DishImage")]
        Dish dish, IFormFile image)
        {
            if (ModelState.IsValid)
            {
               
                if (image != null)
                {
                    string fileName = Guid.NewGuid() + ".jpg";
                    Directory.CreateDirectory(Path.Combine(_webRoot, "images"));
                    var filePath = Path.Combine(_webRoot, "images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await image.CopyToAsync(stream);
                    }
                    filePath = Path.Combine("images", fileName);
                    dish.DishImage = filePath;
                }
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DishTypeId"] = new SelectList(_context.Set<DishType>(), nameof(DishType.Id), nameof(DishType.Name), dish.DishTypeId);
            return View(dish);
        }

        // GET: Dishes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Dish == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish.Include(x => x.DishType).FirstOrDefaultAsync(x => x.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }
            ViewData["DishTypeId"] = new SelectList(_context.Set<DishType>(), nameof(DishType.Id), nameof(DishType.Name), dish.DishTypeId);
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,Name,DishSize,Description,DishStatus,DishTypeId,DishPrice,DishImage")]
        Dish dish, IFormFile image)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image == null)
                    {

                        //var price = string.Format(new CultureInfo("vi-VN"), "{0:C}", Di);


                        var dishExist = await _context.Dish.FirstOrDefaultAsync(x => x.DishId == id);
                        await TryUpdateModelAsync<Dish>(dishExist, "", x => x.Name, x => x.DishSize, x => x.Description, x => x.DishStatus, x => x.DishTypeId, x => x.DishPrice);
                        dishExist.DishImage = dishExist.DishImage;
                    }
                    else
                    {
                        string fileName = Guid.NewGuid() + ".jpg";
                        Directory.CreateDirectory(Path.Combine(_webRoot, "images"));
                        var filePath = Path.Combine(_webRoot, "images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await image.CopyToAsync(stream);
                        }
                        filePath = Path.Combine("images", fileName);
                        dish.DishImage = filePath;
                        _context.Update(dish);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
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
            ViewData["DishTypeId"] = new SelectList(_context.Set<DishType>(), nameof(DishType.Id), nameof(DishType.Name), dish.DishTypeId);
            return View(dish);
        }

        // GET: Dishes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Dish == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .Include(d => d.DishType)
                .FirstOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Dish == null)
            {
                return Problem("Entity set 'FastFoodWebApplicationContext.Dish'  is null.");
            }
            var dish = await _context.Dish.FindAsync(id);
            if (dish != null)
            {
                _context.Dish.Remove(dish);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dish.Any(e => e.DishId == id);
        }
    }
}
