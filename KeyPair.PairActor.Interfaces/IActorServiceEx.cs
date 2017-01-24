using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyPair.PairActor.Interfaces
{
    public interface IActorServiceEx : IService
    {
        Task<bool> ActorExists(ActorId actorId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
