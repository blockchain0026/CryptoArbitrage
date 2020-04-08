using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CryptoArbitrage.Services.Execution.Infrastructure;
using CryptoArbitrage.Services.Execution.WebAPI;
using CryptoArbitrage.Services.Execution.WebAPI.Extensions;
using CryptoArbitrage.Services.Execution.WebAPI.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Execution.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*BuildWebHost(args)
                .Build()
                .MigrateDbContext<ExecutionContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<ExecutionSettings>>();
                    var logger = services.GetService<ILogger<ExecutionContextSeed>>();
                    new ExecutionContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
                })
                .Run();*/

            CreateWebHostBuilder(args)
                .Build() 
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();

        public static IWebHostBuilder BuildWebHost(string[] args)
        {

            return new WebHostBuilder()
                 .UseStartup<Startup>()
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureAppConfiguration((builderContext, config) =>
                 {
                     //config.AddJsonFile("settings.json");
                     config.AddEnvironmentVariables();
                 })
                 .ConfigureLogging((hostingContext, builder) =>
                 {
                     builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     builder.AddConsole();
                     builder.AddDebug();
                 });

        }
    }
}
