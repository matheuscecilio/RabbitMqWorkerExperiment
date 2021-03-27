using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderConsumer.BackgroundServices;
using Serilog;

namespace OrderConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .ReadFrom.Configuration(context.Configuration);

            })
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<SaveOrderConsumer>();
            });
    }
}
