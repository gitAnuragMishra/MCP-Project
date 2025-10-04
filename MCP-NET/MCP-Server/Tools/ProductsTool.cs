using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using MCPSharp;

namespace Tools
{
    public class ProductsTool
    {

        private readonly string _baseURL;
        private readonly HttpUtility _httpUtility;
        public ProductsTool()
        {
            _httpUtility = new HttpUtility();
            _baseURL = ServiceLocator.Instance.GetRequiredService<AppConfig>();
        }



    }
}
