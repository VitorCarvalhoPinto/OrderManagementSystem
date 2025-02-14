using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Entities;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly string _connectionString;
        private readonly string _queueName;

        public OrderService(IOrderRepository orderRepository, IConfiguration configuration) 
        {
            _orderRepository = orderRepository;
            _connectionString = configuration["AzureServiceBus:ConnectionString"];
            _queueName = configuration["AzureServiceBus:QueueName"];
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync() 
        {
            return await _orderRepository.GetOrders();
        }

        public async Task<Order> GetOrdersByIdAsync(Guid id) 
        {
            return await _orderRepository.GetOrderById(id);
        }

        public async Task<Order> CreateOrderAsync(Order order) 
        {
            return await _orderRepository.CreateOrder(order);
        }

        public async Task SendMessageBusSenderAsync(Order order)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);

            string messageBody = JsonSerializer.Serialize(order);
            ServiceBusMessage message = new(messageBody);

            await sender.SendMessageAsync(message);
        }

    }
}
