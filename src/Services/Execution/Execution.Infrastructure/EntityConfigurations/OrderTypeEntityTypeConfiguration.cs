using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class OrderTypeEntityTypeConfiguration
         : IEntityTypeConfiguration<OrderType>
    {
        public void Configure(EntityTypeBuilder<OrderType> orderTypeConfiguration)
        {
            orderTypeConfiguration.ToTable("ordertype", ExecutionContext.DEFAULT_SCHEMA);

            orderTypeConfiguration.HasKey(o => o.Id);

            orderTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
