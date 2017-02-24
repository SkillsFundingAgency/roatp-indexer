namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public interface IIndexSettings<T>
    {
        string IndexesAlias { get; }

        string PauseTime { get; }
    }
}