using Apple.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;
using System.Text;

namespace Apple.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer _azureServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsume(this IApplicationBuilder app)
        {
            _azureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStarted.Register(OnStop);
            return app;
        }

        private static void OnStop()
        {
            _azureServiceBusConsumer.Start();
        }

        private static void OnStart()
        {
            _azureServiceBusConsumer.Start();
        }
    }
}
