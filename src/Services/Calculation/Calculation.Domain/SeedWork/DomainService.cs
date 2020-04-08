using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.SeedWork
{
    public abstract class DomainService
    {
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }


    }
}
