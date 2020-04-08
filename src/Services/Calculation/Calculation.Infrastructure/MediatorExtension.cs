using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<Entity> entitys)
        {
            var domainEntities = entitys.Where(x => x.DomainEvents != null && x.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
