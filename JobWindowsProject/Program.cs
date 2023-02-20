using LoggerService;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobWindowsProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //insatnce mongo
            NO_SQL_CONFIG.Initialize("mongodb://localhost:27017");
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "Gamma Service";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<JobService>();
                })
                //.ConfigureServices(services =>
                //{
                //    services.AddHostedService<JobService>();
                //})
                .Build();
                        await host.RunAsync();
        }
    }
}
