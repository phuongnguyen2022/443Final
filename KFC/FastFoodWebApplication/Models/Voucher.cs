using System.ComponentModel.DataAnnotations;

namespace FastFoodWebApplication.Models
{
    public class Voucher
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }

    }
}
