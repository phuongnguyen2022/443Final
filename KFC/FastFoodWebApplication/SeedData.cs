using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KFCApplication
{
    public class SeedData
    {
        public static async Task SeedRole(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
                await roleManager.CreateAsync(new IdentityRole<int>("Customer"));
            }
        }
    }
}
