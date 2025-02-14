using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrders() 
        {
            return await _context.Order.ToListAsync();
        }

        public async Task<Order> GetOrderById(Guid id) 
        {
            return await _context.Order.FindAsync(id);
        }

        public async Task<Order> CreateOrder(Order order) 
        {
            _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
