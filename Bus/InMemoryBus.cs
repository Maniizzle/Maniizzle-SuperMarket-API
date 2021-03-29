using MediatR;
using Supermarket.API.Domain.Core.Bus;
using Supermarket.API.Domain.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Bus
{
    public class InMemoryBus:IMediatorHandler
    {
        private readonly IMediator mediator;

        public InMemoryBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return mediator.Send(command);
        }
    }
}
