namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IIndexerServiceFactory
    {
        IIndexerService<T> GetIndexerService<T>();
    }
}
