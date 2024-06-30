using Microsoft.AspNetCore.Identity;

namespace KFCApplication.Models
{
    public class AppUser : IdentityUser<int>
    {
        public Profile Profile { get; set; }
    }
}
