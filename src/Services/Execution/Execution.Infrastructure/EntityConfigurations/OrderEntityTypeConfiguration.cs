using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class OrderEntityTypeConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", ExecutionContext.DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Ignore(o => o.DomainEvents);

            orderConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderseq", ExecutionContext.DEFAULT_SCHEMA);



            orderConfiguration.Property(o => o.OrderId)
                .HasMaxLength(200)
                .IsRequired();

            orderConfiguration.HasIndex("OrderId")
              .IsUnique(true);

            orderConfiguration.Property(o => o.ArbitrageId).IsRequired();
            //orderConfiguration.OwnsOne(o => o.Exchange);
            orderConfiguration.Property(o => o.ExchangeId).IsRequired();
            orderConfiguration.Property(o => o.ExchangeOrderId).IsRequired(false);

            /*orderConfiguration.Property<DateTime?>("DateCreated").IsRequired(false);
            orderConfiguration.Property<DateTime?>("DateFilled").IsRequired(false);
            orderConfiguration.Property<DateTime?>("DateCanceled").IsRequired(false);*/

            orderConfiguration.Property<int>("OrderTypeId").IsRequired();
            orderConfiguration.Property<int>("OrderStatusId").IsRequired();

            orderConfiguration.Property(o => o.BaseCurrency).IsRequired();
            orderConfiguration.Property(o => o.QuoteCurrency).IsRequired();
            orderConfiguration.Property(o => o.Price).HasColumnType("decimal(20,8)").IsRequired();
            orderConfiguration.Property(o => o.QuantityTotal).HasColumnType("decimal(20,8)").IsRequired();
            orderConfiguration.Property(o => o.QuantityFilled).HasColumnType("decimal(20,8)").IsRequired();
            orderConfiguration.Property(o => o.CommisionPaid).HasColumnType("decimal(20,8)").IsRequired();


            /*orderConfiguration.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey("BuyerId")
               .OnDelete(DeleteBehavior.Cascade);*/

            orderConfiguration.HasOne(o => o.OrderType)
                .WithMany()
                .HasForeignKey("OrderTypeId");

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusId");
        }
    }
}
