using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    public class ArbitrageTransactionEntityTypeConfiguration : IEntityTypeConfiguration<ArbitrageTransaction>
    {
        public void Configure(EntityTypeBuilder<ArbitrageTransaction> arbitrageTransactionConfiguration)
        {
            arbitrageTransactionConfiguration.ToTable("arbitragetransactions", ExecutionContext.DEFAULT_SCHEMA);

            arbitrageTransactionConfiguration.HasKey(s => s.Id);

            arbitrageTransactionConfiguration.Ignore(s => s.DomainEvents);

            arbitrageTransactionConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("arbitragetransactionsseq", ExecutionContext.DEFAULT_SCHEMA);

            arbitrageTransactionConfiguration.Property(a => a.ArbitrageOrderId)
                .HasMaxLength(200)
                .IsRequired();

            arbitrageTransactionConfiguration.HasIndex(a => a.ArbitrageOrderId)
              .IsUnique(false);

            arbitrageTransactionConfiguration.Property(s=>s.ExchangeId).IsRequired();
            arbitrageTransactionConfiguration.Property<int>("OriginalOrderTypeId").IsRequired();
      
            arbitrageTransactionConfiguration.Property(s => s.BaseCurrency).IsRequired();
            arbitrageTransactionConfiguration.Property(s => s.QuoteCurrency).IsRequired();
            arbitrageTransactionConfiguration.Property(s => s.Price).IsRequired();
            arbitrageTransactionConfiguration.Property(s => s.Volume).IsRequired();
            arbitrageTransactionConfiguration.Property(s => s.CommisionPaid).IsRequired();

            arbitrageTransactionConfiguration.HasOne(s => s.OriginalOrderType)
                .WithMany()
                .HasForeignKey("OriginalOrderTypeId");}
    }
}
