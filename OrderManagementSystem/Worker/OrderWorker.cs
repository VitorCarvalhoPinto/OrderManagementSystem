using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System;
using Domain.Entities;
using Persistence;

namespace OrderManagementSystem.Worker
{
    public class OrderWorker : BackgroundService
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;

        public OrderWorker(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _connectionString = configuration["AzureServiceBus:ConnectionString"];
            _queueName = configuration["AzureServiceBus:QueueName"];
            _serviceScopeFactory = serviceScopeFactory;

            _client = new ServiceBusClient(_connectionString);
            _processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync();
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();

                var order = JsonSerializer.Deserialize<Order>(messageBody);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var existingOrder = await dbContext.Order
                        .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                    Console.WriteLine($"✅ Pedido encontrado: {existingOrder.OrderId} - Status: {existingOrder.Status}");

                    existingOrder.Status = "Processando";
                    await dbContext.SaveChangesAsync();

                    await Task.Delay(5000);

                    existingOrder.Status = "Finalizado";
                    await dbContext.SaveChangesAsync();
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no processamento da mensagem: {ex.Message}");
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
