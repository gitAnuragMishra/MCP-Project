using System.ComponentModel;
using MCPServer1.Helpers;
using MCPSharp;
using Microsoft.SemanticKernel;

namespace Tools
{
    public class ProductsTool
    {

        private readonly string _baseURL;
        private readonly HttpUtility _httpUtility;
        public ProductsTool()
        {
            _httpUtility = new HttpUtility();
            _baseURL = "http://52.66.18.84/";

            //var config = ServiceLocator.ServiceProvider.GetRequiredService<AppConfig>();
            //_baseURL = config.FastAPIURL;

        }

        [McpTool(name: "GetAllProducts", Description = "This function returns all the products. Requires no params")]
        public async Task<string> GetProducts(
            [McpParameter(required: false, description: "PARAMETERLESS")][Description("PARAMETERLESS")] int? i)
        {
            string url = _baseURL + "products";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<long, List<ProductsModel>>(headers, "application/json", url, 1, "GET");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpTool(name: "GetProductById", Description = "Returns a single product by its ID")]
        public async Task<string> GetProductById(
            [McpParameter(required: true, description: "The ID of the product")][Description("The ID of the product")] int productId)
        {
            string url = _baseURL + $"products/{productId}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<long, ProductsModel>(headers, "application/json", url, 1, "GET");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }


        [McpTool(name: "AddProduct", Description = "Adds a new product to the system")]
        public async Task<string> AddProduct(
            [McpParameter(required: true, description: "Product name")][Description("Product name")] string productName,
            [McpParameter(required: true, description: "Product price")][Description("Product price")] double price,
            [McpParameter(required: true, description: "Stock quantity")][Description("Stock quantity")] int stock)
        {
            var payload = new AddProductRequest
            {
                ProductName = productName,
                Price = price,
                Stock = stock
            };

            string url = _baseURL + "products";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<AddProductRequest, Dictionary<string, string>>(headers, "application/json", url, payload, "POST");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpTool(name: "RestockProduct", Description = "Adds stock to a product by product ID")]
        public async Task<string> RestockProduct(
            [McpParameter(required: true, description: "Product ID")][Description("Product ID")] int productId,
            [McpParameter(required: true, description: "Quantity to add")][Description("Quantity to add")] int quantity)
        {
            string url = _baseURL + $"products/{productId}/restock/{quantity}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<object, Dictionary<string, string>>(headers, "application/json", url, 1, "PUT");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpTool(name: "DeleteProduct", Description = "Deletes a product by its ID")]
        public async Task<string> DeleteProduct(
            [McpParameter(required: true, description: "Product ID")][Description("Quantity to add")] int productId)
        {
            string url = _baseURL + $"products/{productId}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<object, Dictionary<string, string>>(headers, "application/json", url, 1, "DELETE");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }


        [McpTool(name: "BuyProduct", Description = "Allows a user to buy a product")]
        public async Task<string> BuyProduct(
            [McpParameter(required: true, description: "User ID")][Description("User ID")] int userId,
            [McpParameter(required: true, description: "Product ID")][Description("Product ID")] int productId,
            [McpParameter(required: true, description: "Quantity to buy")][Description("Quantity to buy")] int quantity)
        {
            var payload = new BuyRequest
            {
                userId = userId,
                productId = productId,
                quantity = quantity
            };

            string url = _baseURL + "buy";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<BuyRequest, Dictionary<string, string>>(headers, "application/json", url, payload, "POST");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpTool(name: "CancelOrder", Description = "Cancels a product order by BoughtID")]
        public async Task<string> CancelOrder(
            [McpParameter(required: true, description: "Bought ID of the order")] int boughtId)
        {
            string url = _baseURL + $"orders/{boughtId}/cancel";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<object, Dictionary<string, object>>(headers, "application/json", url, 1, "PUT");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }


        public class BuyRequest
        {
            public int userId { get; set; }
            public int productId { get; set; }
            public int quantity { get; set; }
        }


        public class ProductsModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Stock { get; set; }
        }

        public class AddProductRequest
        {
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Stock { get; set; }
        }

        public class MessageModel
        {
            public string Message {get; set;}
        }
    }
}
