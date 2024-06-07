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
using System.Globalization;

namespace FastFoodWebApplication.Controllers
{
    public class CartsController : Controller
    {
        private readonly FastFoodWebApplicationContext _context;

        public CartsController(FastFoodWebApplicationContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            string userName = User.Identity.Name;
            var user = _context.Users.Include(u => u.Profile).SingleOrDefault(u => u.UserName == userName);
            if(user!= null)
            {
            var cartItems = await _context.Cart
             .Include(c => c.Dish).Where(c => c.UserId == user.Id)
             .ToListAsync();

            return View(cartItems);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }


        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Dish)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["dishId"] = new SelectList(_context.Dish, "dishid", "name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,DishId,Quantity,size,price,UserId")]
        Cart cart,
         int DishID,
         string size,
         int Price
         )
        {
            string userName = User.Identity.Name;
            var user = _context.Users.Include(u => u.Profile).SingleOrDefault(u => u.UserName == userName);

            //var existingProfile = user.Profile;
            if (ModelState.IsValid)
            {
                var dish = await _context.Dish.SingleOrDefaultAsync(x => x.DishId == DishID);
                decimal price = 0;
                if (size.Equals("S"))
                {
                    price = dish.DishPrice;
                }
                if (size.Equals("M"))
                {
                    price = dish.DishPrice + dish.DishPrice * (decimal)0.4;
                }
                if (size.Equals("L"))
                {
                    price = dish.DishPrice + dish.DishPrice * (decimal)0.8;
                }
                var existingCart = await _context.Cart.Include(x => x.User).SingleOrDefaultAsync(x => x.DishId == DishID && x.size.Equals(size) && x.UserId==user.Id);

                if (existingCart != null)
                {
                    existingCart.Quantity += 1;
                    existingCart.Price = price * existingCart.Quantity;
                    _context.Update(existingCart);
                }
                else
                {
                    cart.Quantity = 1;
                    cart.Price = price;
                    cart.UserId = user.Id;
                    cart.size = size;
                    _context.Cart.Add(cart);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["DishId"] = new SelectList(_context.Dish, "DishId", "Name", cart.DishId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,DishId,Quantity,DishSize")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            ViewData["DishId"] = new SelectList(_context.Dish, "DishId", "Name", cart.DishId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cart == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Dish)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cart == null)
            {
                return Problem("Entity set 'FastFoodWebApplicationContext.Cart'  is null.");
            }
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartId == id);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(
            int dishId, int quantity, string size)
        {
            // Retrieve the cart item based on dish ID and user
            string userName = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == userName);

            var cartItem = await _context.Cart
                .Include(c => c.Dish)
                .FirstOrDefaultAsync(c => c.DishId == dishId && c.UserId == user.Id);

            if (cartItem != null)
            {
                // Update the quantity and size
                cartItem.Quantity = quantity;
                cartItem.size = size;

                // Recalculate the price based on the updated quantity and size
                var dish = await _context.Dish.SingleOrDefaultAsync(x => x.DishId == dishId);
                cartItem.Price = CalculatePrice(dish.DishPrice, size, quantity);
                var total = string.Format(new CultureInfo("vi-VN"), "{0:C}", cartItem.Price);

                await _context.SaveChangesAsync();
                // You can return a response if needed
                return Json(new { Success = true, UpdatedPrice = total });

            }

            // Return an error response if the cart item is not found
            return Json(new { Success = false, UpdatedPrice = cartItem.Price });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCartBySize(
            int dishId, string size, int cartID)
        {
            // Retrieve the cart item based on dish ID and user
            string userName = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == userName);

            // Retrieve all cart items for the specified dish and user
            var cartItems = await _context.Cart
                .Include(c => c.Dish)
                .Where(c => c.DishId == dishId && c.UserId == user.Id)
                .ToListAsync();
            var newCartItem = cartItems.FirstOrDefault(c => !string.Equals(c.size, size, StringComparison.OrdinalIgnoreCase) && c.CartId == cartID);

            if (newCartItem != null)
            {
                var existingCartItem = cartItems.FirstOrDefault(c => string.Equals(c.size, size, StringComparison.OrdinalIgnoreCase));
                if (existingCartItem != null)
                {
                    // Item with the same ID and size exists, increment quantity and update price
                    existingCartItem.Quantity += newCartItem.Quantity;
                    existingCartItem.Price = CalculatePrice(newCartItem.Dish.DishPrice, size, existingCartItem.Quantity);
                    var total = string.Format(new CultureInfo("vi-VN"), "{0:C}", existingCartItem.Price);
                    // Remove the original item
                    _context.Cart.Remove(newCartItem);
                    await _context.SaveChangesAsync();
                    return Json(new { Success = true, UpdatedPrice = total, cartReturn = 1});
                }
                else
                {
                    // Size has changed, update the item's size and recalculate the price
                    var quantity = newCartItem.Quantity;
                    newCartItem.size = size;
                    newCartItem.Price = CalculatePrice(newCartItem.Dish.DishPrice, size, quantity);
                    var total = string.Format(new CultureInfo("vi-VN"), "{0:C}", newCartItem.Price);
                    await _context.SaveChangesAsync();
                    return Json(new { Success = true, UpdatedPrice = total });
                }


            }

            // Return an error response if the cart item is not found
            return Json(new { Success = false, ErrorMessage = "Cart item not found." });

        }
        // calculate the price depends on the size
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

        public JsonResult GetTotal()
        {
            string userName = User.Identity.Name;
            var user = _context.Users.SingleOrDefault(u => u.UserName == userName);

            decimal total = _context.Cart
                .Where(c => c.UserId == user.Id)
                .Sum(c => c.Price);
            var totalFomat = string.Format(new CultureInfo("vi-VN"), "{0:C}", total);
            return Json(totalFomat);
        }

    }
}
