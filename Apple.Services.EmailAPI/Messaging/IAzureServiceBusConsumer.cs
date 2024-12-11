namespace Apple.Services.EmailAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        public Task Start();
        public Task Stop();
    }
}