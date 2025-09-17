using MCPSharp;

namespace Tools
{
    public class ProductTools
    {
        [McpTool(name: "list_products", Description = "This function will list all the products")]
        public static string ListAll()
        {
            return "This is the listing mehod";
        }
    }
}
