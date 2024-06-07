
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace FastFoodWebApplication.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OderDate { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 3)")]
        [Range(0, (double)decimal.MaxValue)]
        public decimal TotalPrice { get; set; }
        public string shipping_status { get; set; }
        public int UserId { get; set; }        
        public AppUser User { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber {  get; set; }
        public string voucherCode { get; set; }

    }
}
