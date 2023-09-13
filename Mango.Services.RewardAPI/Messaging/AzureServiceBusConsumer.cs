using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Models.DTOs;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubsription;
        private readonly IConfiguration _config;
        private readonly RewardService _rewardService;
        private ServiceBusProcessor _rewardsprocessor;

        public AzureServiceBusConsumer(IConfiguration config, RewardService rewardService)
        {
            _config  = config;
            serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardsSubsription = _config.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardsprocessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubsription);
            _rewardService  = rewardService;
        }

        public async Task Start()
        {
            _rewardsprocessor.ProcessMessageAsync += OnRewardsReceived;
            _rewardsprocessor.ProcessErrorAsync += ErrorHandler;
            await _rewardsprocessor.StartProcessingAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // Typically we send email about error in starting the service.... Please implement it later using SendGrid
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnRewardsReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsDTO rewardsDTO = JsonConvert.DeserializeObject<RewardsDTO>(body);
            try
            {
                _rewardService.UpdateRewards(rewardsDTO);
                await args.CompleteMessageAsync(args.Message); // This will automatically remove the message from the queue
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _rewardsprocessor.StopProcessingAsync();
            await _rewardsprocessor.DisposeAsync();
        }
    }
}
