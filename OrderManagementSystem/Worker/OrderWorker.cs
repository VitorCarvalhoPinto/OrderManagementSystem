using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Persistence;

namespace OrderManagementSystem.Worker
{
    public class OrderWorker : BackgroundService
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrderWorker(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) 
        {
            _connectionString = configuration["AzureServiceBus:ConnectionString"];
            _queueName = configuration["AzureServiceBus:QueueName"];
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += async args =>
            {
                var order = JsonSerializer.Deserialize<Order>(args.Message.Body.ToString());

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var existingOrder = await dbContext.Order.FindAsync(order.OrderId);
                    if (existingOrder != null)
                    {
                        existingOrder.Status = "Processando";
                        await dbContext.SaveChangesAsync();

                        await Task.Delay(5000);

                        existingOrder.Status = "Finalizado";
                        await dbContext.SaveChangesAsync();
                    }
                }

                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Erro: {args.Exception}");
                return Task.CompletedTask;

            };
            await processor.StartProcessingAsync();
        }
    }
}
