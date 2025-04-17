namespace DUTEG.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string UserIdentifier { get; set; } // Can be user ID, cookie/session key
    }
}
