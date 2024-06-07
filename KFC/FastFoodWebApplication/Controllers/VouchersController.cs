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
    public class VouchersController : Controller
    {
        private readonly FastFoodWebApplicationContext _context;

        public VouchersController(FastFoodWebApplicationContext context)
        {
            _context = context;
        }

        // GET: Vouchers
        public async Task<IActionResult> Index()
        {
              return View(await _context.Voucher.ToListAsync());
        }
        public async Task<IActionResult> ListVouchers()
        {
            var vouchers = await _context.Voucher.ToListAsync();
            return PartialView("ListVouchers", vouchers);
        }


        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Voucher == null)
            {
                return NotFound();
            }

            var voucher = await _context.Voucher
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // GET: Vouchers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Code,Description,Name,Amount,Quantity")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.Code = GenerateVoucherCode();
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: Vouchers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Voucher == null)
            {
                return NotFound();
            }

            var voucher = await _context.Voucher.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        
        public async Task<IActionResult> SaveVoucher(int codeID)
        {
            string userName = User.Identity.Name;
            var user = _context.Users.Include(u => u.Profile).SingleOrDefault(u => u.UserName == userName);

            if(user!= null)
            {
                var sql = $"INSERT INTO UserVoucher (UserId, VoucherId, VoucherStatus) VALUES ({user.Id},{codeID},1)";
                await _context.Database.ExecuteSqlRawAsync(sql);
                var voucher = await _context.Voucher.FirstOrDefaultAsync(c => c.ID == codeID);
                voucher.Quantity -= 1;
                _context.Update(voucher);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Code,Description,Name,Amount,Quantity")] Voucher voucher)
        {
            if (id != voucher.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.ID))
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
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Voucher == null)
            {
                return NotFound();
            }

            var voucher = await _context.Voucher
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Voucher == null)
            {
                return Problem("Entity set 'FastFoodWebApplicationContext.Voucher'  is null.");
            }
            var voucher = await _context.Voucher.FindAsync(id);
            if (voucher != null)
            {
                _context.Voucher.Remove(voucher);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(int id)
        {
          return _context.Voucher.Any(e => e.ID == id);
        }

        private string GenerateVoucherCode()
        {
            int length = 6;
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();

            char[] voucherCode = new char[length];
            for (int i = 0; i < length; i++)
            {
                voucherCode[i] = characters[random.Next(characters.Length)];
            }

            return new string(voucherCode);
        }
    }
}
