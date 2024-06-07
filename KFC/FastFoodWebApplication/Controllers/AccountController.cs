using FastFoodWebApplication.Data;
using FastFoodWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using FastFoodWebApplication.Areas.Identity.Pages.Account;
using FastFoodWebApplication.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FastFoodWebApplication.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace FastFoodWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly FastFoodWebApplicationContext _context;
        private readonly String _webRoot;
        public AccountController(FastFoodWebApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _webRoot = env.WebRootPath;
        }
        public IActionResult Index()
        {
            String userName = User.Identity.Name;
            var user = _context.Users.Include(u => u.Profile).SingleOrDefault(u => u.UserName == userName);
                var existingProfile = user.Profile;

            
            return View(existingProfile);
        }
        public IActionResult AccessDenied()
        {

            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInput model, [FromServices] SignInManager<AppUser> signInManager, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("/Identity/Account/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }
        public IActionResult Register(String returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInput model, [FromServices] IUserStore<AppUser> _userStore,
            [FromServices] UserManager<AppUser> _userManager,
            [FromServices] IEmailSender _emailSender, [FromServices] SignInManager<AppUser> _signInManager,
            string returnUrl = null)
        {
            var _emailStore = GetEmailStore(_userManager, _userStore);
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateUser();

               

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user.Profile = new Profile { UserSpend = 0 };
                    var userId = await _userManager.GetUserIdAsync(user);
                    _context.Update(user);
                    _context.SaveChanges();
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                    pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                 
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

           

            return View(model);
        }
        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        private IUserEmailStore<AppUser> GetEmailStore(UserManager<AppUser> _userManager, IUserStore<AppUser> _userStore)
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
        public async Task<IActionResult> Logout([FromServices] SignInManager<AppUser> _signInManager,
              [FromServices] ILogger<LogoutModel> _logger)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return View();
        }


        public async Task<IActionResult> UpdateProfile()
        {
            var username = User.Identity.Name;
            var currentUser = await _context.Users.Include(x => x.Profile).FirstOrDefaultAsync(x => x.UserName == username);
            return View(currentUser?.Profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([Bind("Avatar,FirstName,LastName,Gender,Dob,Address,Phone,Nationality")]
        Profile profile, IFormFile avatar)
        {
            String userName = User.Identity.Name;
            var user = _context.Users.Include(u => u.Profile).SingleOrDefault(u => u.UserName == userName);
            profile.UserId = user.Id;
            var existingProfile = user.Profile;
            if (ModelState.IsValid)
            {
                if (avatar != null)
                {
                    //Save file to physical storage
                    string fileName = Guid.NewGuid() + ".jpg";
                    Directory.CreateDirectory(Path.Combine(_webRoot, "images"));
                    var filePath = Path.Combine(_webRoot, "images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await avatar.CopyToAsync(stream);
                    }
                    filePath = Path.Combine("images", fileName);
                    profile.Avatar = filePath;
                }

                if (existingProfile == null)
                {
                    _context.Add(profile);
                }
                else
                {
                    await TryUpdateModelAsync<Profile>(existingProfile, "", p => p.LastName, p => p.FirstName, p => p.Gender, p => p.Dob, p => p.Address, p => p.Phone, p => p.Nationality);
                    if (avatar != null)
                    {
                        existingProfile.Avatar = profile.Avatar;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }
        //[Authorize(Roles ="admin")]
        public IActionResult ManageRole([FromServices] FastFoodWebApplicationContext context)
        {
            var users = context.Users.Include(x =>x.Profile).ToList();
            ViewBag.Users = users;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(IdentityUserRole<int> model, [FromServices] FastFoodWebApplicationContext context, [FromServices] UserManager<AppUser> userManager)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            var roles = await userManager.GetRolesAsync(new AppUser { Id = model.UserId });
            var role = await context.Roles.SingleOrDefaultAsync(x => x.Id == model.RoleId);

            await userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await userManager.AddToRoleAsync(user, role.Name);
            return PartialView("UpdateRoleResult");
        }
        public IActionResult GetRole(int id, [FromServices] FastFoodWebApplicationContext context, [FromServices] RoleManager<IdentityRole<int>> roleManager)
        {
            var users = context.Users.SingleOrDefault(x => x.Id == id);
            var roles = roleManager.Roles.ToList();
            var currentRole = context.UserRoles.FirstOrDefault(x => x.UserId == id);
            ViewBag.UserName = users.UserName;
            ViewBag.UserId = id;
            ViewBag.Roles = new SelectList(roles, "Id", "Name", currentRole?.RoleId);
            return PartialView("_RoleForm", currentRole);
        }
    }
}
