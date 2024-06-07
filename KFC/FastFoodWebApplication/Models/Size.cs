namespace FastFoodWebApplication.Models
{
    public class Size
    {
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        public DishSize DishSize { get; set; }
    }
}
