using System.Configuration;
using Topshelf;

namespace FundEstimateWebClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            string serviceDisplayName = ConfigurationManager.AppSettings["ServiceDisplayName"];
            string serviceDescription = ConfigurationManager.AppSettings["ServiceDescription"];

            TopshelfExitCode serviceRunner = HostFactory.Run(config =>
            {
                config.Service<TelegramBotService>(service =>
                {
                    service.ConstructUsing(name => new TelegramBotService());
                    service.WhenStarted(serviceConfig => serviceConfig.Start());
                    service.WhenStopped(serviceConfig => serviceConfig.Stop());
                });
                config.RunAsLocalSystem();

                config.SetServiceName(serviceName);
                config.SetDisplayName(serviceDisplayName);
                config.SetDescription(serviceDescription);
            });

            int exitCode = (int)Convert.ChangeType(serviceRunner, serviceRunner.GetTypeCode());

            Environment.ExitCode = exitCode;
        }
    }
}
