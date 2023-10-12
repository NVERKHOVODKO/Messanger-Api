using Microsoft.AspNetCore.SignalR;

namespace TestTask.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string chatId, string user, string message)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddToGroup(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task RemoveFromGroup(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }
    }
}