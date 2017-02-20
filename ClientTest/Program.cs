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
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.ServiceFabric.Actors.Query;
using System.Threading;
using System.Fabric;

namespace ClientTest
{
    class Program
    {
        //static string url = "fabric:/WebReferenceApplication/InventoryService";
        static string url = "fabric:/KeyPair.WebService/PairActorService";
        static void Main(string[] args)
        {
            //https://disqus.com/home/discussion/thewindowsazureproductsite/reliable_service_backup_and_restore_microsoft_azure/
           
           // Guid userID = new Guid("23103127-A5DF-495A-B672-C041D5166D89");
            //Guid userID = new Guid("ce8d8d29-0d07-4943-954e-3f573659a77b");
            ////Guid userID = new Guid("7C9F2082-3965-44D3-87B6-5433BA727707");
            

            ///var actorService = ActorServiceProxy.Create<IActorServiceEx>(new Uri("fabric:/KeyPairServiceFabric/PairActorService"), (new ActorId(userID)));
            ////Task<bool> exists = actorService.ActorExists(new ActorId(userID));
            ////bool result = exists.Result;
            ////ContentServiceReference.ContentServiceClient client = new ContentServiceReference.ContentServiceClient();
            //IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId(userID), new Uri("fabric:/KeyPairServiceFabric/PairActorService"));
            //IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId(userID), new Uri(url));
            //Task<Dictionary<int, Pairs>> pairData = actor.GetKeyValuePair();
            //Dictionary<int, Pairs> pair = pairData.Result;

            PerformDataLossUseSelectorSample();
            //IActorService myActorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/KeyPairServiceFabric/PairActorService"), new ActorId(userID));
            //myActorServiceProxy.DeleteActorAsync(new ActorId(userID), new System.Threading.CancellationToken()).GetAwaiter().GetResult();


            //ContinuationToken continuationToken = null;
            //List<ActorInformation> activeActors = new List<ActorInformation>();

            //do
            //{
            //    PagedResult<ActorInformation> page = myActorServiceProxy
            //                 .GetActorsAsync(continuationToken, new CancellationToken()).GetAwaiter().GetResult();

            //    activeActors.AddRange(page.Items.Where(x => x.IsActive));

            //    continuationToken = page.ContinuationToken;
            //}
            //while (continuationToken != null);
            //foreach (ActorInformation info in activeActors)
            //{
            //    Console.WriteLine("INFO:" + info.ActorId + ":" + info.IsActive);
            //}


            //if (!result)
            //{
            //    ContentServiceReference.Pairs pair = client.GetKeyValuePair(userID, null);
            //    DataContractSerializer dcs = new DataContractSerializer(typeof(ContentServiceReference.Pairs));
            //    string s = SerializationHelper<ContentServiceReference.Pairs>.Serialize(pair);
            //    Pairs p = SerializationHelper<KeyPair.PairActor.Interfaces.Model.Pairs>.Deserialize(s);

            //    Task pairData = actor.SetKeyValuePair(p);
            //    //Pairs pairs = pairData.Result;
            //}
            //else
            //{
            //    Task<Pairs> pairData = actor.GetKeyValuePair();
            //    Pairs pair = pairData.Result;
            //}

            //IPairActor actor = ActorProxy.Create<IPairActor>(new ActorId("23103127-A5DF-495A-B672-C041D5166D89"), new Uri("fabric:/KeyPairServiceFabric/PairActorService"));
            //Task<Pairs> pairData = actor.GetKeyValuePair();
            //Pairs pairs = pairData.Result;
            //ContentServiceReference.ContentServiceClient client = new ContentServiceReference.ContentServiceClient();
            Console.ReadLine();
        }
        static async Task PerformDataLossUseSelectorSample()
        {
            // Create a unique operation id for the command below
            Guid operationId = Guid.NewGuid();

            // Note: Use the appropriate overload for your configuration
            FabricClient fabricClient = new FabricClient();

            // The name of the target service
            Uri targetServiceName = new Uri(url);

            // Use a PartitionSelector that will have the Fault Injection and Analysis Service choose a random partition of “targetServiceName”
            PartitionSelector partitionSelector = PartitionSelector.RandomOf(targetServiceName);
            
            // Start the command.  Retry OperationCanceledException and all FabricTransientException's.  Note when StartPartitionDataLossAsync completes
            // successfully it only means the Fault Injection and Analysis Service has saved the intent to perform this work.  It does not say anything about the progress
            // of the command.
            while (true)
            {
                try
                {
                    await fabricClient.TestManager.StartPartitionDataLossAsync(operationId, partitionSelector, DataLossMode.FullDataLoss).ConfigureAwait(false);
                    break;
                }
                catch (OperationCanceledException)
                {
                }
                catch (FabricTransientException)
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception '{0}'", e);
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(1.0d)).ConfigureAwait(false);
            }

            PartitionDataLossProgress progress = null;

            // Poll the progress using GetPartitionDataLossProgressAsync until it is either Completed or Faulted.  In this example, we're assuming
            // the command won't be cancelled.

            while (true)
            {
                try
                {
                    progress = await fabricClient.TestManager.GetPartitionDataLossProgressAsync(operationId).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    continue;
                }
                catch (FabricTransientException)
                {
                    continue;
                }

                if (progress.State == TestCommandProgressState.Completed)
                {
                    Console.WriteLine("Command '{0}' completed successfully", operationId);

                    Console.WriteLine("Printing progress.Result:");
                    Console.WriteLine("  Printing selected partition='{0}'", progress.Result.SelectedPartition.PartitionId);

                    break;
                }
                else if (progress.State == TestCommandProgressState.Faulted)
                {
                    // If State is Faulted, the progress object's Result property's Exception property will have the reason why.
                    Console.WriteLine("Command '{0}' failed with '{1}', SelectedPartition {2}", operationId, progress.Result.Exception, progress.Result.SelectedPartition);
                    break;
                }
                else
                {
                    Console.WriteLine("Command '{0}' is currently Running", operationId);
                }

                await Task.Delay(TimeSpan.FromSeconds(1.0d)).ConfigureAwait(false);
            }
        }
    }
    public static class SerializationHelper<T>
    {
        /// <summary>
        /// Deserialize the xml element
        /// </summary>
        /// <param name="xmlString">a string variable</param>
        /// <param name="xmlData">an object</param>
        /// <returns>return serializer as an object</returns>
        public static T Deserialize(string xmlString, T xmlData)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(xmlData.GetType());

                // Create a string reader
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    // Use the Deserialize method to restore the object's state.
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch
            {
                return default(T);
            }
        }

        public static T Deserialize(string xmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch
            {
                return default(T);
            }
        }
        public static bool CanDeserialize(string xmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlReader reader = XmlReader.Create(new StringReader(xmlString));
                return serializer.CanDeserialize(reader);
            }
            catch
            {
                return false;
            }

        }
        public static string Serialize(object o, string nsPrefix, string defaultNamespace)
        {
            try
            {
                bool omitXmlDeclaration = true;

                XmlSerializer serializer = new XmlSerializer(typeof(T), defaultNamespace);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(nsPrefix, defaultNamespace);

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
                    xmlWriterSettings.Indent = false;

                    using (XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringWriter, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, o, ns);
                    }

                    return stringWriter.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Serialize(object o)
        {
            return Serialize(o, true);
        }

        public static string Serialize(object o, bool omitXmlDeclaration)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
                    xmlWriterSettings.Indent = false;

                    using (XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringWriter, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, o);
                    }

                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// Serialize the xml element and replace the root node
        /// </summary>
        /// <param name="index">an int variable</param>
        /// <param name="childNode">an Object</param>
        /// <param name="renameNode">a string variable</param>
        /// <returns>return xmlDoc as a string</returns>
        public static string Serialize(T[] childNode, string renameNode)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder stringBuilder = new StringBuilder();

            // Omitting the XML version
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = false;
            xmlSettings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlSettings))
            {
                if (childNode != null)
                {
                    // If the parent node is not null then append child nodes
                    XmlSerializer serializer = new XmlSerializer(childNode.GetType());
                    serializer.Serialize(writer, childNode);
                    xmlDoc.LoadXml(stringBuilder.ToString());
                    XmlElement newXmlDocElement = xmlDoc.CreateElement(renameNode);
                    while (xmlDoc.DocumentElement.HasChildNodes)
                    {
                        newXmlDocElement.AppendChild(xmlDoc.DocumentElement.ChildNodes[0]);
                    }
                    xmlDoc.RemoveChild(xmlDoc.DocumentElement);
                    xmlDoc.AppendChild(newXmlDocElement);
                }
                else
                {
                    //if the parent node is null then replace a elment as root
                    XmlElement xmlNodeElement = xmlDoc.CreateElement(renameNode);
                    xmlDoc.AppendChild(xmlNodeElement);
                }
            }
            return xmlDoc.InnerXml;
        }
    }
}
