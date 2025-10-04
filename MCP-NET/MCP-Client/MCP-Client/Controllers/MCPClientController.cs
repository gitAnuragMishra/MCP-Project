using MCP_Client.Mcp_Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCP_Client.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class MCPClientController : ControllerBase
    {
        private readonly ConnectServer _connectServer;

        public MCPClientController(ConnectServer connectServer)
        {
            _connectServer = connectServer;
        }

        [HttpPost]
        public async Task<IActionResult> ChatMCP([FromBody] string query)
        {
            var response = await _connectServer.InvokeMCP(query);   
            if (response == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the request.");
            }
            return Ok(response);
        }
    }
}
