using MediatR;
using System;

namespace ProjectX.Core.IntegrationEvents
{
    public interface IIntegrationEvent : IRequest, IHasTransaction 
    {
        public Guid Id { get; set; }
    }
}
