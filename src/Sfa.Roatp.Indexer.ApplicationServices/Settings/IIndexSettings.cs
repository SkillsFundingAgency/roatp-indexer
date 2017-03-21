namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public interface IIndexSettings<T>
    {
        string IndexesAlias { get; }

        int PauseTime { get; }
    }
}