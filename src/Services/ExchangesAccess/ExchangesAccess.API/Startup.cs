using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CryptoArbitrage.EventBus;
using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.EventBusRabbitMQ;
using CryptoArbitrage.EventBusServiceBus;
using CryptoArbitrage.IntegrationEventLogEF.Services;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.EventHandling;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using CryptoArbitrage.Services.ExchangesAccess.API.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NSwag.AspNetCore;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace ExchangesAccess.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
    sp => (DbConnection c) => new IntegrationEventLogService(c));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Arbitrage Exchanges Access API Document",
                    Version = "v1",
                    Description = "",
                    TermsOfService = "None"
                });
                /*var xmlFile = "ExchangesAccess.API.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);*/
            });




            services.Configure<ExchangeApiSetting>(Configuration.GetSection("ExchangeAPIs"));

            services.AddSingleton<IExchangeApiClient, ExchangeApiClient>();
            services.AddSingleton<IExchangeMarketData, ExchangeMarketData>();
            services.AddSingleton<IExchangeBalanceData, ExchangeBalanceData>();
            services.AddSingleton<IWebsocketService, WebsocketService>();
            services.AddSingleton<IHostedService, WebsocketHostService>();
            services.AddSingleton<IExchangeQueries, ExchangeQueries>();



            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                //var settings = sp.GetRequiredService<IOptions<CatalogSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }
                else
                {
                    factory.UserName = "guest";
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }
                else
                {
                    factory.Password = "guest";
                }
                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            RegisterEventBus(services);

            var container = new ContainerBuilder();
            container.Populate(services);

            // if you have handlers/events in other assemblies
            // services.AddMediatR(typeof(SomeHandler).Assembly, 
            //                     typeof(SomeOtherHandler).Assembly);
            services.AddMediatR();

            return new AutofacServiceProvider(container.Build());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{Id?}"
                    );
            });

            /*app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;
            });*/

            app.UseSwaggerUi3WithApiExplorer(s =>
            {
                s.GeneratorSettings.DefaultPropertyNameHandling =
                PropertyNameHandling.CamelCase;
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api/docs";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exchanges Access API V1");
            });

            ConfigureEventBus(app);

            /*app.UseSignalR(
                routes =>
                {
                    routes.MapHub<OrderBookHub>("/orderBookHub");
                }
                );*/

        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });


            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();


            #region Integration Events Handlers
            services.AddTransient<ExchangeOrderCreatedIntegrationEventHandler>();
            services.AddTransient<ArbitrageOrderStartedIntegrationEventHandler>();
            services.AddTransient<TimeForUpdateBalanceIntegrationEventHandler>();
            #endregion
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            #region Integration Events
            eventBus.Subscribe
                <ExchangeOrderCreatedIntegrationEvent,
                ExchangeOrderCreatedIntegrationEventHandler>();
            eventBus.Subscribe
                <ArbitrageOrderStartedIntegrationEvent,
                ArbitrageOrderStartedIntegrationEventHandler>();
            eventBus.Subscribe
                <TimeForUpdateBalanceIntegrationEvent,
                TimeForUpdateBalanceIntegrationEventHandler>();
            #endregion
        }
    }
}
