using CryptoArbitrage.Services.Execution.Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.EntityConfigurations
{
    class ClientRequestEntityTypeConfiguration
       : IEntityTypeConfiguration<ClientRequest>
    {
        public void Configure(EntityTypeBuilder<ClientRequest> requestConfiguration)
        {
            requestConfiguration.ToTable("requests", ExecutionContext.DEFAULT_SCHEMA);
            requestConfiguration.HasKey(cr => cr.Id);
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }
}
