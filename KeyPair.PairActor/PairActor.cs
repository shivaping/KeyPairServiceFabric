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
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;
using KeyPair.PairActor.Utility;

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

        public Task<bool> DelKeyValuePair(int pairId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DelKeyValuePairByKey(int pairId, string pairKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DelKeyValuePairByKeys(int pairId, string[] pairKeys)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TransferGuestKeyValuePair(Guid parentuserId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, string.Format("Actor activated {0}.", this.Id.ToString()));

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization
            ConditionalValue<Pairs> state = await this.StateManager.TryGetStateAsync<Pairs>(KeyPairStatePropertyName);

            if (!state.HasValue)
            {
                ContentService.ContentServiceClient client = new ContentService.ContentServiceClient();
                ContentService.Pairs pair = client.GetKeyValuePair(this.Id.GetGuidId(), null);
                DataContractSerializer dcs = new DataContractSerializer(typeof(ContentService.Pairs));
                string s = SerializationHelper<ContentService.Pairs>.Serialize(pair);
                Pairs p = SerializationHelper<KeyPair.PairActor.Interfaces.Model.Pairs>.Deserialize(s);

                ActorEventSource.Current.ActorMessage(this, s);
                await this.StateManager.SetStateAsync<Pairs>(KeyPairStatePropertyName, p);
                ActorEventSource.Current.ActorMessage(this, "Pairs: State initialized");
            }


            //if (actors.Result != null)
            //{
            //    return this.StateManager.TryAddStateAsync(KeyPairState, Pairs);
            //}
            //    return this.StateManager.TryAddStateAsync(KeyPairState, Pairs);
            return;
        }
        protected override async Task OnDeactivateAsync()
        {
            // ConditionalValue<Pairs> state = await this.StateManager.TryGetStateAsync<Pairs>(KeyPairStatePropertyName);
            //this.StateManager = null;
            ActorEventSource.Current.ActorMessage(this, string.Format("Actor DE Activated {0}.", this.Id.ToString()));
            await base.OnDeactivateAsync();
            return;
        }
    }
}