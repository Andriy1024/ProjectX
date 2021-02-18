using MediatR;
using System;

namespace ProjectX.Core.IntegrationEvents
{
    public interface IIntegrationEvent : INotification 
    {
        public Guid Id { get; set; }
    }
}
