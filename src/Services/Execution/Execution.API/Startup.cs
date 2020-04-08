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
using CryptoArbitrage.Services.Execution.API;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.API.Infrastructure;
using CryptoArbitrage.Services.Execution.API.Infrastructure.AutofacModules.ApplicationModule;
using CryptoArbitrage.Services.Execution.API.Infrastructure.AutofacModules.MediatorModule;
using CryptoArbitrage.Services.Execution.API.IntegrationEvents.EventHandling;
using CryptoArbitrage.Services.Execution.API.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Execution.API
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IExecutionIntegrationEventService, ExecutionIntegrationEventService>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddEntityFrameworkSqlServer()
             .AddDbContext<ExecutionContext>(options =>
             {
                 options.UseSqlServer(Configuration["ConnectionString"],
                     sqlServerOptionsAction: sqlOptions =>
                     {
                         var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                         sqlOptions.MigrationsAssembly(assemblyName);
                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         sqlOptions.CommandTimeout(3 * 60);
                         
                     });
             },
                 ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
             );

            services.Configure<ExecutionSettings>(Configuration);


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


            services.AddSingleton<IExchangeApiRequestFrequenciesControlService, ExchangeApiRequestFrequenciesControlService>();


            //Configure event bus.
            RegisterEventBus(services);
            var container = new ContainerBuilder();
            container.Populate(services);

            services.AddMediatR();

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            ConfigureEventBus(app);

            // Seed data through our custom class
            ExecutionContextSeed.SimpleSeedAsync(app).Wait();
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });

            }
            else
            {
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
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();


            #region Integration Events Handlers
            services.AddTransient<ProfitRoomFoundedIntegrationEventHandler>();
            services.AddTransient<ArbitrageOrderSubmittedToExchangeFailedIntegrationEventHandler>();
            services.AddTransient<ArbitrageOrderSubmittedToExchangeSuccessIntegrationEventHandler>();
            services.AddTransient<ExchangeOrderCanceledIntegrationEventHandler>();
            services.AddTransient<ExchangeOrderFullExecutedIntegrationEventHandler>();
            services.AddTransient<ExchangeOrderPartiallyExecutedIntegrationEventHandler>();
            services.AddTransient<ProfitRoomFoundedWithEnoughBalancesIntegrationEventHandler>();
            #endregion
        }


        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            #region Integration Events

            eventBus.Subscribe<ProfitRoomFoundedIntegrationEvent,
                              ProfitRoomFoundedIntegrationEventHandler>();
            eventBus.Subscribe<ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent,
                  ArbitrageOrderSubmittedToExchangeFailedIntegrationEventHandler>();
            eventBus.Subscribe<ArbitrageOrderSubmittedToExchangeSuccessIntegrationEvent,
                  ArbitrageOrderSubmittedToExchangeSuccessIntegrationEventHandler>();
            eventBus.Subscribe<ExchangeOrderCanceledIntegrationEvent,
                   ExchangeOrderCanceledIntegrationEventHandler>();
            eventBus.Subscribe<ExchangeOrderFullExecutedIntegrationEvent,
                   ExchangeOrderFullExecutedIntegrationEventHandler>();
            eventBus.Subscribe<ExchangeOrderPartiallyExecutedIntegrationEvent,
                  ExchangeOrderPartiallyExecutedIntegrationEventHandler>();
            eventBus.Subscribe<ProfitRoomFoundedWithEnoughBalancesIntegrationEvent,
                   ProfitRoomFoundedWithEnoughBalancesIntegrationEventHandler>();
            #endregion
        }
    }
}
