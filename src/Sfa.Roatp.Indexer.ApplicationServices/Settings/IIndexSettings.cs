namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public interface IIndexSettings<T>
    {
        string IndexesAlias { get; }

        int PauseAfterIndexing { get; }

        string StatusCakeUrl { get; }
    }
}