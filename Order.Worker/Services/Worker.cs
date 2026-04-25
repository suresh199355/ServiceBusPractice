using Azure.Messaging.ServiceBus;
using Order.Worker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Order.Worker.Services
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;
        private ServiceBusClient _client;

        public Worker(IConfiguration config)
        {
            _config = config;
            _client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartQueueListener();
            await StartTopicListener();
        }

        private async Task StartQueueListener()
        {
            var processor = _client.CreateProcessor(
                _config["ServiceBus:QueueName"],
                new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += async args =>
            {
                var body = Encoding.UTF8.GetString(args.Message.Body);
                var order = JsonSerializer.Deserialize<OrderMessage>(body);

                Console.WriteLine($"QUEUE → Processing Order: {order!.OrderId}");

                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine(args.Exception.Message);
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
        }

        private async Task StartTopicListener()
        {
            var processor = _client.CreateProcessor(
                _config["ServiceBus:TopicName"],
                "orders-sub",
                new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += async args =>
            {
                var body = Encoding.UTF8.GetString(args.Message.Body);
                var order = JsonSerializer.Deserialize<OrderMessage>(body);

                Console.WriteLine($"TOPIC → Notification for Order: {order!.OrderId}");

                await args.CompleteMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine(args.Exception.Message);
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
        }
    }
}
