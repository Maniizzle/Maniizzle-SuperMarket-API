using Supermarket.API.Domain.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Domain.Core.Command
{
    public abstract class Command:Message
    {
        public DateTime TimeStamp { get; set; }
        public Command()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
