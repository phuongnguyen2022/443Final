namespace FastFoodWebApplication.Models
{
    public class UserVoucher
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set;}
        public int VoucherStatus { get; set; }

    }
}
