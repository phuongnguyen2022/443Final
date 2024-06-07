using System.ComponentModel.DataAnnotations;

namespace FastFoodWebApplication.Models
{
    public class OrderDetail
    {
       
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string size { get; set; }
    }
}
