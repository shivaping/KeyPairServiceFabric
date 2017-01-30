using KeyPair.PairActor.Interfaces;
using KeyPair.PairActor.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
namespace KeyPairAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        // GET api/values 
        public string Get()
        {
            Guid userID = new Guid("7B761CD6-C114-4F58-AC2C-42B905E50844");
            //Guid userID = new Guid("7C9F2082-3965-44D3-87B6-5433BA727707");
            string url = "fabric:/KeyPair.WebService/PairActorService";

            //var actorService = ActorServiceProxy.Create<IActorServiceEx>(new Uri("fabric:/KeyPairServiceFabric/PairActorService"), (new ActorId(userID)));
            //Task<bool> exists = actorService.ActorExists(new ActorId(userID));
            //bool result = exists.Result;
            //ContentServiceReference.ContentServiceClient client = new ContentServiceReference.ContentServiceClient();
            //IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId(userID), new Uri("fabric:/KeyPairServiceFabric/PairActorService"));
            IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId(userID), new Uri(url));
            Task<Pairs> pairData = actor.GetKeyValuePair();
            Pairs pair = pairData.Result;
            string value = Newtonsoft.Json.JsonConvert.SerializeObject(pair);
            return  value ;
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
