using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class OrderStatusEntityTypeConfiguration
       : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> orderStatusConfiguration)
        {
            orderStatusConfiguration.ToTable("orderstatus", ExecutionContext.DEFAULT_SCHEMA);

            orderStatusConfiguration.HasKey(o => o.Id);

            orderStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
