using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class StopLossSettingEntityTypeConfiguration
         : IEntityTypeConfiguration<StopLossSetting>
    {
        public void Configure(EntityTypeBuilder<StopLossSetting> stopLossSettingConfiguration)
        {
            stopLossSettingConfiguration.ToTable("stoplosssettings", ExecutionContext.DEFAULT_SCHEMA);

            stopLossSettingConfiguration.HasKey(s => s.Id);

            stopLossSettingConfiguration.Ignore(s => s.DomainEvents);

            stopLossSettingConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("stoplosssettingseq", ExecutionContext.DEFAULT_SCHEMA);

            stopLossSettingConfiguration.OwnsOne(s => s.Exchange);


            stopLossSettingConfiguration.HasMany(s => s.SlipPrices)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
