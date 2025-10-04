using System.ComponentModel;
using MCPServer2.Helpers;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using static Program;

namespace Tools
{
    [McpServerToolType]
    public class ProductsTool
    {

        private readonly string _baseURL;
        private readonly HttpUtility _httpUtility;
        public ProductsTool(IHttpClientFactory httpClientFactory, IOptions<AppConfig> appConfig)
        {
            _httpUtility = new HttpUtility(httpClientFactory);

            _baseURL = appConfig?.Value?.FastAPIURL ?? "http://52.66.18.84/";

        }

        [McpServerTool, Description("This function returns all the products. Requires no params")]
        public async Task<string> GetProducts([Description("PARAMETERLESS")] int? i)
        {
            string url = _baseURL + "products";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<long, List<ProductsModel>>(headers, "application/json", url, 1, "GET");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpServerTool, Description("Returns a single product by its ID")]
        public async Task<string> GetProductById([Description("The ID of the product")] int productId)
        {
            string url = _baseURL + $"products/{productId}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<long, ProductsModel>(headers, "application/json", url, 1, "GET");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }


        [McpServerTool, Description("Adds a new product to the system")]
        public async Task<string> AddProduct(
            [Description("Product name")] string productName,
            [Description("Product price")] double price,
            [Description("Stock quantity")] int stock)
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

        [McpServerTool, Description("Adds stock to a product by product ID")]
        public async Task<string> RestockProduct(
            [Description("Product ID")] int productId,
            [Description("Quantity to add")] int quantity)
        {
            string url = _baseURL + $"products/{productId}/restock/{quantity}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<object, Dictionary<string, string>>(headers, "application/json", url, 1, "PUT");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }

        [McpServerTool, Description("Deletes a product by its ID")]
        public async Task<string> DeleteProduct(
            [Description("Product ID")] int productId)
        {
            string url = _baseURL + $"products/{productId}";
            var headers = new Dictionary<string, string>();
            var resp = await _httpUtility.GetHttpCallAsync<object, Dictionary<string, string>>(headers, "application/json", url, 1, "DELETE");

            return resp != null ? JsonHelper.Serialize(resp) : "";
        }


        [McpServerTool, Description("Allows a user to buy a product")]
        public async Task<string> BuyProduct(
            [Description("User ID")] int userId,
            [Description("Product ID")] int productId,
            [Description("Quantity to buy")] int quantity)
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

        [McpServerTool, Description("Cancels a product order by BoughtID")]
        public async Task<string> CancelOrder(
            [Description ("Bought ID of the order")] int boughtId)
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
