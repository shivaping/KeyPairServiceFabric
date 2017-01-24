using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using KeyPair.PairActor.Interfaces.Model;

namespace KeyPair.PairActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IPairActor : IActor
    {
        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken);

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task SetCountAsync(int count, CancellationToken cancellationToken);


        Task<Pairs> GetKeyValuePair();


     

        Task SetKeyValuePair(Pairs pairs);

        Task<bool> DelKeyValuePair(int pairId, CancellationToken cancellationToken);


        Task<bool> DelKeyValuePairByKey(int pairId, string pairKey, CancellationToken cancellationToken);


        Task<bool> DelKeyValuePairByKeys(int pairId, string[] pairKeys, CancellationToken cancellationToken);


        Task<bool> TransferGuestKeyValuePair(Guid parentuserId, CancellationToken cancellationToken);
    }
}
