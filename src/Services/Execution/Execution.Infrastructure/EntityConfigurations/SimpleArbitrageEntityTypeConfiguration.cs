using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class SimpleArbitrageEntityTypeConfiguration
        : IEntityTypeConfiguration<SimpleArbitrage>
    {
        public void Configure(EntityTypeBuilder<SimpleArbitrage> simpleArbitrageConfiguration)
        {
            simpleArbitrageConfiguration.ToTable("simplearbitrages", ExecutionContext.DEFAULT_SCHEMA);

            simpleArbitrageConfiguration.HasKey(s => s.Id);

            simpleArbitrageConfiguration.Ignore(s => s.DomainEvents);

            simpleArbitrageConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("simplearbitrageseq", ExecutionContext.DEFAULT_SCHEMA);



            simpleArbitrageConfiguration.Property(s => s.ArbitrageId)
                .HasMaxLength(200)
                .IsRequired();

            simpleArbitrageConfiguration.HasIndex("ArbitrageId")
              .IsUnique(true);

            simpleArbitrageConfiguration.OwnsOne(s => s.BuyOrder, b =>
            {
                b.Property(p => p.Price).HasColumnType("decimal(20,8)");
                b.Property(p => p.Quantity).HasColumnType("decimal(20,8)");
            });
            /*simpleArbitrageConfiguration.OwnsOne(s => s.BuyOrder,
                b =>
                {
                    b.Property<int>("OrderTypeId").IsRequired();
                    b.HasOne(a => a.OrderType)
                    .WithMany()
                    .HasForeignKey("OrderTypeId");
                });*/
            simpleArbitrageConfiguration.OwnsOne(s => s.SellOrder,b=> {
                b.Property(p => p.Price).HasColumnType("decimal(20,8)");
                b.Property(p => p.Quantity).HasColumnType("decimal(20,8)");
            });
            /*simpleArbitrageConfiguration.OwnsOne(s => s.SellOrder,
                b =>
                {
                    b.OwnsOne(a => a.Exchange);
                    b.Property<int>("ArbitrageOrderTypeId").IsRequired();
                    b.HasOne(a => a.OrderType)
                    .WithMany()
                    .HasForeignKey("ArbitrageOrderTypeId");
                });*/

            simpleArbitrageConfiguration.Property(s => s.EstimateProfits).HasColumnType("decimal(20,8)").IsRequired();
            simpleArbitrageConfiguration.Property(s => s.ActualProfits).HasColumnType("decimal(20,8)").IsRequired();

            simpleArbitrageConfiguration.OwnsOne(s => s.ArbitrageData, b => {
                b.Property(p => p.OriginalBaseCurrencyQuantity).HasColumnType("decimal(20,8)");
                b.Property(p => p.OriginalQuoteCurrencyQuantity).HasColumnType("decimal(20,8)");
                b.Property(p => p.FinalBaseCurrencyQuantity).HasColumnType("decimal(20,8)");
                b.Property(p => p.FinalQuoteCurrencyQuantity).HasColumnType("decimal(20,8)");
            });
            simpleArbitrageConfiguration.Property<int>("SimpleArbitrageStatusId").IsRequired();
            simpleArbitrageConfiguration.Property(s => s.IsSuccess).IsRequired();
            simpleArbitrageConfiguration.Property(s => s.FailureReason);

            simpleArbitrageConfiguration.HasMany(s => s.Transactions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = simpleArbitrageConfiguration.Metadata.FindNavigation(nameof(SimpleArbitrage.Transactions));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            simpleArbitrageConfiguration.HasOne(s => s.Status)
                .WithMany()
                .HasForeignKey("SimpleArbitrageStatusId");


        }
    }
}
