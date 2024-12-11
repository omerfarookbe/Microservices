using Apple.Services.EmailAPI.Models.Dtos;
using Apple.Services.EmailAPI.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Apple.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string _servicebusConnectionString;
        private readonly string _servicebusQueueName;

        private ServiceBusProcessor _servicebusprocessor;

        private readonly IConfiguration _configuration;

        public readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _servicebusConnectionString = configuration.GetValue<string>("SBConnectionString");
            _servicebusQueueName = configuration.GetValue<string>("ServiceBus:QueueName");
            _emailService = emailService;

            var client = new ServiceBusClient(_servicebusConnectionString);
            _servicebusprocessor = client.CreateProcessor(_servicebusQueueName);
        }

        public async Task Start()
        {
            _servicebusprocessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _servicebusprocessor.ProcessErrorAsync += ErrorHandler;
            _servicebusprocessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            _servicebusprocessor.StopProcessingAsync();
            _servicebusprocessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                await _emailService.EmailCartAndLog(cartDto);
                args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
