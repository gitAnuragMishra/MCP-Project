using AIChatService;
using Microsoft.AspNetCore.Mvc;

namespace MCP_Client_Web.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class AIChatController : Controller
    {
        private readonly GeminiChatServiceWeb _chatService;

        public AIChatController(GeminiChatServiceWeb chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody]string prompt)
        {
            var reply = await _chatService.SendMessageAsync(prompt);
            return Content(reply);
        }
    }
}
