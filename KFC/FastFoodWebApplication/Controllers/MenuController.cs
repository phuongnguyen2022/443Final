using KFCApplication.Data;
using KFCApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using X.PagedList;

namespace KFCApplication.Controllers
{
    public class MenuController : Controller
    {
        public readonly KFCApplicationContext _context;
        public MenuController( KFCApplicationContext context)
        {

            _context = context;
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(int? DishTypeId, string searchString, string sortOrder,int? page)
        {const int pageSize = 3;
            var dishes = await _context.Dish.Include(d => d.DishType).ToListAsync();

            if (DishTypeId != null)
            {
                dishes = dishes.Where(x => x.DishTypeId == DishTypeId).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                dishes = dishes.Where(d => d.Name.ToLower().Contains(searchString) || d.Description.ToLower().Contains(searchString)).ToList();
            }
            var dishSizes = Enum.GetValues(typeof(DishSize)).Cast<DishSize>();
            // Sorting logic
            ViewBag.PriceSortParam = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            switch (sortOrder)
            {
                case "price_desc":
                    dishes = dishes.OrderByDescending(d => d.DishPrice).ToList();
                    break;
                default:
                    dishes = dishes.OrderBy(d => d.DishPrice).ToList();
                    break;
            }
            // Paging logic
            int pageNumber = page ?? 1;
            IPagedList<Dish> pagedDishes = dishes.ToPagedList(pageNumber, pageSize);

            ViewData["Dishes"] = pagedDishes;
            ViewData["DishType"] = await _context.DishType.ToListAsync();
            ViewData["active"] = DishTypeId;
            ViewData["DishSizes"] = dishSizes;
            ViewData["searchString"] = searchString;
            return View();

        }


        // GET: Dishes/Details/5
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
            ViewBag.Price = dish.DishPrice;
            return View(dish);
        }

        private decimal CalculatePrice(decimal basePrice, string size, int quantity)
        {
            decimal sizePrice = 0;

            if (size == "M")
            {
                sizePrice = basePrice * 0.4m;
            }
            else if (size == "L")
            {
                sizePrice = basePrice * 0.8m;
            }
            return (basePrice + sizePrice) * quantity;
        }
    }
}
