using Microsoft.AspNetCore.Server;
using Microsoft.AspNetCore.SignalR;

namespace OrderManagementSystem.OrderHub
{
    public class OrderHub : Hub
    {
        public async Task NotifyOrderStatusUpdated(Guid orderId, string status) 
        {
            await Clients.All.SendAsync("OrderStatusUpdated", orderId, status);
        }
    }
}
