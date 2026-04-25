using Azure.Messaging.ServiceBus;
using Order.Api.Models;
using System.Text.Json;

namespace Order.Api.Services;

public class ServiceBusPublisher(IConfiguration config)
{
    private readonly IConfiguration? _config = config;
    private readonly ServiceBusClient? _serviceBusClient = new ServiceBusClient(config["ServiceBus:ConnectionString"]);

    public async Task PublishOrder(OrderMessage orderMessage)
    { 
        string queueName = _config["ServiceBus:QueueName"] ?? "orders";
        string topicName = _config["ServiceBus:TopicName"] ?? "orders-topic";

        var  json = JsonSerializer.Serialize(orderMessage);

        var queueSender = _serviceBusClient?.CreateSender(queueName);
        if (queueSender is not null)
        {
            await queueSender.SendMessageAsync(new ServiceBusMessage(json));
        }

        //Send To Topic 
        var  topicSender = _serviceBusClient?.CreateSender(topicName);
        if (topicSender is not null)
        {
            await topicSender.SendMessageAsync(new ServiceBusMessage(json));
        }
    }

}

