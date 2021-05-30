using AirlineSendAgent.App;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AirlineSendAgent
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddScoped<IAppHost, AppHost>();
                    services.AddScoped<IWebhookClient, WebhookClient>();
                    services.AddDbContext<SendAgentDbContext>(conf =>
                        conf.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddHttpClient();
                }).Build();


            host.Services.GetService<IAppHost>().Run();
        }
    }
}
