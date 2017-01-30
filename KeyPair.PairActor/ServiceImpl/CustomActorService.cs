using KeyPair.PairActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyPair.PairActor.ServiceImpl
{
    internal class CustomActorService : ActorService
    {
        public CustomActorService(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, Func<ActorService, ActorId, ActorBase> actorFactory = null, Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null, IActorStateProvider stateProvider = null, ActorServiceSettings settings = null)
            : base(context, actorTypeInfo)
        {
        }
        protected override Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(this.Context, "inside OnDataLossAsync for KeyPairState Service");
            return base.OnDataLossAsync(restoreCtx, cancellationToken);
        }
        //public async Task<bool> ActorExists(ActorId actorId, CancellationToken cancellationToken)
        //{
        //    const int batchSize = 1000;
        //    ContinuationToken token = null;
        //    do
        //    {
        //        var actors = await StateProvider.GetActorsAsync(batchSize, token, cancellationToken);
        //        if (actors.Items.Contains(actorId))
        //        {
        //            return true;
        //        }
        //        token = actors.ContinuationToken;
        //    } while (token != null);
        //    return false;
        //}
        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "inside RunAsync for KeyPairState Service");
                return Task.WhenAll(new List<Task>() { this.PeriodicTakeBackupAsync(cancellationToken) });
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "RunAsync Failed, {0}", e);
                throw;
            }

        }
        private async Task PeriodicTakeBackupAsync(CancellationToken cancellationToken)
        {
            long backupsTaken = 0;
            //this.SetupBackupManager();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                {
                    await Task.Delay(TimeSpan.FromSeconds(24*60*60));
                    //BackupDescription backupDescription = new BackupDescription(BackupOption.Full, this.BackupCallbackAsync);
                    //await this.BackupAsync(backupDescription, TimeSpan.FromHours(1), cancellationToken);
                    //backupsTaken++;
                    ServiceEventSource.Current.ServiceMessage(this.Context, "Backup {0} taken", backupsTaken);
                    const int batchSize = 1000;
                    ContinuationToken token = null;
                    do
                    {
                        var actors = await StateProvider.GetActorsAsync(batchSize, token, cancellationToken);
                        foreach (var item in actors.Items)
                        {
                            ServiceEventSource.Current.ServiceMessage(this.Context, "UserID {0} ", item.GetGuidId());
                        }

                        token = actors.ContinuationToken;
                    } while (token != null);
                }
            }
        }
    }
}
