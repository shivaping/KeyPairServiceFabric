using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyPair.PairActor.Interfaces.Model;
using KeyPair.PairActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorService = ActorServiceProxy.Create<IActorServiceEx>(new Uri("fabric:/KeyPairServiceFabric/PairActorService"), (new ActorId("23103127-A5DF-495A-B672-C041D5166D89")));
            Task<bool> exists = actorService.ActorExists(new ActorId("23103127-A5DF-495A-B672-C041D5166D89"));
            bool result = exists.Result;
            //IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId("23103127-A5DF-495A-B672-C041D5166D89"), new Uri("fabric:/KeyPairServiceFabric/PairActorService"));
            //Task<Pairs> pairData = actor.GetKeyValuePair();
            //Pairs pairs = pairData.Result;
            //ContentServiceReference.ContentServiceClient client = new ContentServiceReference.ContentServiceClient();
        }
    }
}
