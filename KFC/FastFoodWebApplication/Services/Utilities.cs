namespace FastFoodWebApplication.Services
{
    public static class Utilities
    {
        public static string FormatNumber(this decimal number)
        {
            /*            return number.ToString("N2");
            */
            return number.ToString("#,### đ");
        }
    }
}
