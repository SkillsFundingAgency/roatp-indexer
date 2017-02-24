using System;
using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IIndexerService<T>
    {
        Task CreateScheduledIndex(DateTime scheduledRefreshDateTime);
    }
}