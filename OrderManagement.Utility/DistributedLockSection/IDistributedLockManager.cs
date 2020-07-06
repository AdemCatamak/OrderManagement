using System;
using System.Threading.Tasks;

namespace OrderManagement.Utility.DistributedLockSection
{
    public interface IDistributedLockManager
    {
        void Lock(string key, Action action);
        Task LockAsync(string key, Func<Task> action);
    }
}