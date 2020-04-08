using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class SimpleArbitrageStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<SimpleArbitrageStatus>
    {
        public void Configure(EntityTypeBuilder<SimpleArbitrageStatus> simpleArbitrageStatusConfiguration)
        {
            simpleArbitrageStatusConfiguration.ToTable("simplearbitragestatus", ExecutionContext.DEFAULT_SCHEMA);

            simpleArbitrageStatusConfiguration.HasKey(o => o.Id);

            simpleArbitrageStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            simpleArbitrageStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
