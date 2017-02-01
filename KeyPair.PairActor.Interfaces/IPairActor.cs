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
        Task<Dictionary<int, Pairs>> GetKeyValuePair();
        Task SetKeyValuePair(Pairs pairs);
        Task<bool> DelKeyValuePair(int pairId);
        Task<bool> DelKeyValuePairByKey(int pairId, string pairKey);
        Task<bool> DelKeyValuePairByKeys(int pairId, string[] pairKeys);
        Task<bool> TransferGuestKeyValuePair(Guid parentuserId);
    }
}
