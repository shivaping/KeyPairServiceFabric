using KeyPair.PairActor.Interfaces;
using KeyPair.PairActor.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors;
using System.Data.SqlClient;
using System.Data;

namespace KeyPairAPI.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        string url = "fabric:/KeyPair.WebService/PairActorService";
        // GET api/values 
        public string GetKeyValuePair(string userID, int? pairId)
        {
            IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId(userID), new Uri(url));
            Task<Dictionary<int, Pairs>> pairData = actor.GetKeyValuePair();
            var pair = pairData.Result;
            string returnValue = string.Empty;
            if (pairId.HasValue)
                returnValue = Newtonsoft.Json.JsonConvert.SerializeObject(pair[pairId.Value].Items);
            returnValue = Newtonsoft.Json.JsonConvert.SerializeObject(pair.Values);
            return returnValue;
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
