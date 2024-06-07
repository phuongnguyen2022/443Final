using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FastFoodWebApplication.Models
{
    public class MenuViewModel
    {
        public List<Dish> Dishes { get; set; }
        public SelectList DishTypes { get; set; }

        public string DishType { get; set; }   
        public string SearchString { get; set;}
    }
}
