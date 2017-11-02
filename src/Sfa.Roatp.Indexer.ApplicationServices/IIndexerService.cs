using System;
using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IIndexerService<T>
    {
        void CheckRoatpAndCreateIndexAndUpdateAlias(DateTime scheduledRefreshDateTime);
    }
}