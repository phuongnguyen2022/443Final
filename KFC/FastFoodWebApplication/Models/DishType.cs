using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastFoodWebApplication.Models
{
    public class DishType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}