using AIChatService;
using Microsoft.AspNetCore.Mvc;

namespace MCP_Client.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class AIChatController : Controller
    {
        private readonly GeminiChatService _chatService;

        public AIChatController(GeminiChatService chatService)
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
