using ChatMeeting.Core.Domain.Dtos;
using ChatMeeting.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatMeeting.API.Hubs;

[Authorize]
public class MessageHub : Hub
{
    private readonly IUserConnectionService _userConnectionService;
    private readonly IChatService _chatService;
    private string _username => _userConnectionService.GetClaimValue(Context.User, ClaimTypes.NameIdentifier);
    private string _userId => _userConnectionService.GetClaimValue(Context.User, JwtRegisteredClaimNames.Jti);
    private const string _mainChat = "Global";
    public MessageHub(IUserConnectionService userConnectionService, IChatService chatService)
    {
        _userConnectionService = userConnectionService;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        _userConnectionService.AddConnection(_username, Context.ConnectionId);
        await JoinChatAsync(_mainChat);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _userConnectionService.RemoveConnection(_username);
        await LeaveChatAsync(_mainChat);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToChat(string chatId, string message)
    {
        await Clients.Group(_mainChat).SendAsync("ReceiveMessage", _username, message);
        await _chatService.SaveMessageInKafkaAsync(CreateMessageDto(chatId, message));
    }

    private MessageDto CreateMessageDto(string chatId, string message) => new MessageDto()
    {
        MessageId = Guid.NewGuid(),
        MessageText = message,
        ChatId = Guid.Parse(chatId),
        Sender = _username,
        SenderId = Guid.Parse(_userId),
        CreatedAt = DateTime.Now
    };

    private async Task JoinChatAsync(string chatName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
    }

    private async Task LeaveChatAsync(string chatName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
    }
}
