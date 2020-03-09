using System;
using System.Threading.Tasks;

namespace L1.Jobs.RecurringTasks
{
    public interface IBackupTaiXiuTask
    {
        Task Run();
    }

    public sealed class BackupTaiXiuTask : IBackupTaiXiuTask
    {
        public Task Run()
        {
            throw new NotImplementedException();
        }
    }
}