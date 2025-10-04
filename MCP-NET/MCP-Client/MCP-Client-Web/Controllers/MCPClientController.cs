using CommonFunctions;
using Microsoft.AspNetCore.Mvc;

namespace MCP_Client_Web.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class MCPClientController : ControllerBase
    {
        private readonly ConnectServerHttp _connectServer;

        public MCPClientController(ConnectServerHttp connectServer)
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
