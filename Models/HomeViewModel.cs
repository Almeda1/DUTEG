namespace DUTEG.Models
{
    public class HomeViewModel
    {
        public required List<Product> Products { get; set; } = new List<Product>();
        public required List<CartItem> Services { get; set; } = new List<CartItem>();
    }
}
