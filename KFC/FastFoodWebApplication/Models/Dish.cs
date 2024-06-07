using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FastFoodWebApplication.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        public DishSize DishSize { get; set; }

        public string Description { get; set; }
        public int DishStatus { get; set; }
        [ForeignKey(nameof(DishType))]
        [Display(Name = "DishType")]
        public int DishTypeId { get; set; }
        public DishType DishType { get; set; }
        [DataType(DataType.Currency)]
    
        [Range(0, (double)decimal.MaxValue)]
 
        public decimal DishPrice { get; set; }
        public string DishImage { get; set; }

        [NotMapped]
        public string FormattedDishPrice
        {
            get
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:C}", DishPrice);
            }
        }
    }
}
