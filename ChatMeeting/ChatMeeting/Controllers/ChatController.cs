using ChatMeeting.Core.Domain.Exceptions;
using ChatMeeting.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatMeeting.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILogger<ChatController> logger, IChatService chatService)
    {
        _logger = logger;
        _chatService = chatService;
    }

    [HttpPost("GetPaginatedChat")]
    public async Task<IActionResult> GetPaginatedChat(string chatName, int pageNumber, int pageSize)
    {
        try
        {
            var chat = await _chatService.GetPaginatedChatAsync(chatName, pageNumber, pageSize);
            return Json(chat);
        } catch (ChatNotFoundException ex)
        {
            return Conflict(ex.Message);
        } catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error occured");
            return StatusCode(500, ex.Message);
        }
    }
}
