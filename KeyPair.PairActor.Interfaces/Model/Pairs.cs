using System;
using System.Runtime.Serialization;


namespace KeyPair.PairActor.Interfaces.Model
{
    [Serializable]
    [DataContract(Name = "Pairs", Namespace = "")]
    public class Pairs
    {
        [DataMember(Name = "Items", IsRequired = true)]
        public PairsPair[] Items { get; set; }

        [DataMember(Name = "FromCache", IsRequired = true)]
        public bool FromCache { get; set; }
    }
}
