using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.ApplicationServices.Queue
{
    public interface IGenericControlQueueConsumer
    {
        Task CheckMessage<T>()
            where T : IMaintainSearchIndexes;
    }
}