using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using KeyPair.PairActor.Interfaces;
using KeyPair.PairActor.Interfaces.Model;
using Microsoft.ServiceFabric.Data;
using System.Fabric.Description;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Actors.Query;

namespace KeyPair.PairActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class PairActor : Actor, IPairActor
    {
        private const string KeyPairStatePropertyName = "KeyPairState";
        /// <summary>
        /// Initializes a new instance of PairActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public PairActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task SetKeyValuePair(Pairs pairs)
        {
            await this.StateManager.SetStateAsync<Pairs>(KeyPairStatePropertyName, pairs);
        }
        public async Task<Pairs> GetKeyValuePair()
        {
            return await this.StateManager.GetStateAsync<Pairs>(KeyPairStatePropertyName);
        }
        public async Task<Pairs> ActorExists()
        {
            return await this.StateManager.GetStateAsync<Pairs>(KeyPairStatePropertyName);
        }
        public Task<bool> DelKeyValuePair(int pairId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DelKeyValuePairByKey(int pairId, string pairKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DelKeyValuePairByKeys(int pairId, string[] pairKeys, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        

        public Task<bool> TransferGuestKeyValuePair(Guid parentuserId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization
            
            //    return this.StateManager.TryAddStateAsync(KeyPairState, Pairs);
            return this.StateManager.TryAddStateAsync("count", 0);

        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> IPairActor.GetCountAsync(CancellationToken cancellationToken)
        {
            return this.StateManager.GetStateAsync<int>("count", cancellationToken);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task IPairActor.SetCountAsync(int count, CancellationToken cancellationToken)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
        }

    }

    
    internal class CustomActorService : ActorService, IActorServiceEx
    {
        public CustomActorService(StatefulServiceContext context, ActorTypeInformation actorTypeInfo)
            : base(context, actorTypeInfo )
        {
        }

        public async Task<bool> ActorExists(ActorId actorId, CancellationToken cancellationToken)
        {
            const int batchSize = 1000;
            ContinuationToken token = null;
            do
            {
                var actors = await StateProvider.GetActorsAsync(batchSize, token, cancellationToken);
                if (actors.Items.Contains(actorId))
                {
                    return true;
                }
                token = actors.ContinuationToken;
            } while (token != null);
            return false;
        }
    }
}
