using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Infrastructure
{
    public class ExecutionContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "execution";
        public DbSet<Order> Orders { get; set; }
        public DbSet<SimpleArbitrage> SimpleArbitrages { get; set; }
        public DbSet<StopLossSetting> StopLossSettings { get; set; }
        public DbSet<SimpleArbitrageStatus> SimpleArbitrageStatus { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<ArbitrageTransaction> AribtrageTransactions { get; set; }

        private readonly IMediator _mediator;

        private ExecutionContext(DbContextOptions<ExecutionContext> options) : base(options) { }

        public ExecutionContext(DbContextOptions<ExecutionContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("ExecutionContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SimpleArbitrageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StopLossSettingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SimpleArbitrageStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ArbitrageTransactionEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed throught the DbContext will be commited
            var result = await base.SaveChangesAsync();

            await _mediator.DispatchDomainEventsAsync(this);

            return true;
        }
    }

    public class ExecutionContextDesignFactory : IDesignTimeDbContextFactory<ExecutionContext>
    {
        public ExecutionContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public ExecutionContext CreateDbContext(string[] args)
        {

            /*var optionsBuilder = new DbContextOptionsBuilder<ExecutionContext>()
                .UseSqlServer("Server=.;Initial Catalog=CryptoArbitrage.Services.ExecutionDb;Integrated Security=true");*/
            var optionsBuilder = new DbContextOptionsBuilder<ExecutionContext>()
                .UseSqlServer("Server=sql.data;Database=cryptoarbitrage;User Id=sa;Password=1Secure*Password1;",
                sqlServerOptionsAction: sqlOptions =>
                {
                    var assemblyName = "Execution.API";
                    sqlOptions.MigrationsAssembly(assemblyName);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }
                );
            return new ExecutionContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task Send(IRequest request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.CompletedTask;
            }
        }
    }
}
