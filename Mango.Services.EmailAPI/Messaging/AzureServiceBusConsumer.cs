using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.DTOs;
using Mango.Services.EmailAPI.Service;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subsription;
        private readonly ServiceBusProcessor _orderCreated_Email_Processor;

        private ServiceBusProcessor _emailprocessor;

        public AzureServiceBusConsumer(IConfiguration config, EmailService emailService)
        {
            _config  = config;
            _emailService = emailService;
            serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            orderCreated_Topic = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subsription = _config.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");


            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailprocessor = client.CreateProcessor(emailCartQueue);
            _orderCreated_Email_Processor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subsription);
        }

        public async Task Start()
        {
            _emailprocessor.ProcessMessageAsync += OnEmailCartReceived;
            _emailprocessor.ProcessErrorAsync += ErrorHandler;
            await _emailprocessor.StartProcessingAsync();

            _orderCreated_Email_Processor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _orderCreated_Email_Processor.ProcessErrorAsync += ErrorHandler;
            await _orderCreated_Email_Processor.StartProcessingAsync();
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            OrderPlacedEmailNotificationDTO orderPlacedDTO = JsonConvert.DeserializeObject<OrderPlacedEmailNotificationDTO>(body);
            try
            {
                // try to log email
                _emailService.LogOrderPlaced(orderPlacedDTO);
                await args.CompleteMessageAsync(args.Message); // This will automatically remove the message from the queue
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // Typically we send email about error in starting the service.... Please implement it later using SendGrid
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            ShoppingCartDTO cartDTO = JsonConvert.DeserializeObject<ShoppingCartDTO>(body);
            try
            {
                // try to log email
                _emailService.EmailAndLog(cartDTO);
                await args.CompleteMessageAsync(args.Message); // This will automatically remove the message from the queue
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _emailprocessor.StopProcessingAsync();
            await _emailprocessor.DisposeAsync();

            await _orderCreated_Email_Processor.StopProcessingAsync();
            await _orderCreated_Email_Processor.DisposeAsync();
        }
    }
}
