using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace FastFoodWebApplication.Models
{
    public enum Gender
    {
        Male, Female, Other
    }
    public enum Nationality
    {
        VietNamese, ThaiLand, England, American, Australian
    }
    public class Profile
    {
        [Key]
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public string Avatar { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public Gender Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Date of birth")]
        public DateTime Dob { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 20)]
        public string Address { get; set; }
        [RegularExpression("[0-9]{10}")]
        public string Phone { get; set; }
        public Nationality Nationality { get; set; }
        
        [Column(TypeName = "decimal(18, 3)")]
        [Range(0, (double)decimal.MaxValue)]
        public decimal UserSpend { get; set; }
    }
}