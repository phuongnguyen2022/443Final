using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FastFoodWebApplication
{
    public class SeedData
    {
        public static async Task SeedRole(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                await roleManager.CreateAsync(new IdentityRole<int>("admin"));
                await roleManager.CreateAsync(new IdentityRole<int>("user"));
            }
        }
    }
}
