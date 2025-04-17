using DUTEG.Models;
using Newtonsoft.Json;

   namespace DUTEG.Services
   {
        public static class FileRepository
        {
            private static readonly string productsFilePath = "DataFolder/products.json";
            private static readonly string servicesFilePath = "Data/services.json";
            private static readonly string cartFilePath = "Data/cart.json";

            // Load products from the JSON file
            public static List<Product> LoadProducts()
            {
                if (!File.Exists(productsFilePath))
                    return new List<Product>();

                var json = File.ReadAllText(productsFilePath);
                return JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }

            // Save products to the JSON file
            public static void SaveProducts(List<Product> products)
            {
                var json = JsonConvert.SerializeObject(products, Formatting.Indented);
                File.WriteAllText(productsFilePath, json);
            }

            // Load services from the JSON file
            public static List<CartItem> LoadServices()
            {
                if (!File.Exists(servicesFilePath))
                    return new List<CartItem>();

                var json = File.ReadAllText(servicesFilePath);
                return JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new List<CartItem>();
            }

            // Save services to the JSON file
            public static void SaveServices(List<CartItem> services)
            {
                var json = JsonConvert.SerializeObject(services, Formatting.Indented);
                File.WriteAllText(servicesFilePath, json);
            }

            // Load cart items from the JSON file
            public static List<ShoppingCartItem> LoadShoppingCart()
            {
                if (!File.Exists(cartFilePath))
                    return new List<ShoppingCartItem>();

                var json = File.ReadAllText(cartFilePath);
                return JsonConvert.DeserializeObject<List<ShoppingCartItem>>(json) ?? new List<ShoppingCartItem>();
            }

            // Save cart items to the JSON file
            public static void SaveShoppingCart(List<ShoppingCartItem> cartItems)
            {
                var json = JsonConvert.SerializeObject(cartItems, Formatting.Indented);
                File.WriteAllText(cartFilePath, json);
            }
        }
}
