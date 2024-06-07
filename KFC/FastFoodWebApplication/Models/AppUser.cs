using Microsoft.AspNetCore.Identity;

namespace FastFoodWebApplication.Models
{
    public class AppUser:IdentityUser<int>
    {
        public Profile Profile { get; set; }
    }
}
