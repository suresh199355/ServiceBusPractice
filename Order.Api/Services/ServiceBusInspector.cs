using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace Order.Api.Services;

public class ServiceBusInspector
{
    private readonly ServiceBusClient _client;
    private readonly IConfiguration _config;

    public ServiceBusInspector(IConfiguration config)
    {
        _config = config;
        _client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
    }

    public async Task<List<string>> PeekQueueMessagesAsync(int maxMessages = 10)
    {
        var queueName = _config["ServiceBus:QueueName"] ?? "orders";
        var receiver = _client.CreateReceiver(queueName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        var messages = new List<string>();
        var peeked = await receiver.PeekMessagesAsync(maxMessages);
        foreach (var msg in peeked)
        {
            messages.Add(Encoding.UTF8.GetString(msg.Body));
        }

        return messages;
    }

    public async Task<List<string>> PeekSubscriptionMessagesAsync(string subscriptionName = "orders-sub", int maxMessages = 10)
    {
        var topicName = _config["ServiceBus:TopicName"] ?? "orders-topic";
        var receiver = _client.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        var messages = new List<string>();
        var peeked = await receiver.PeekMessagesAsync(maxMessages);
        foreach (var msg in peeked)
        {
            messages.Add(Encoding.UTF8.GetString(msg.Body));
        }

        return messages;
    }
}
