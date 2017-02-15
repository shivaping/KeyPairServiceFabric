

namespace KeyPair.PairActor.BackUp.Interface
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;

    public interface IBackupStore
    {
        long backupFrequencyInSeconds { get; }

        Task ArchiveBackupAsync(BackupInfo backupInfo, CancellationToken cancellationToken);

        Task<string> RestoreLatestBackupToTempLocation(CancellationToken cancellationToken);

        Task DeleteBackupsAsync(CancellationToken cancellationToken);
    }
}
