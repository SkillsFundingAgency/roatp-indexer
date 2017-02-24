namespace Sfa.Roatp.Indexer.WorkerRole.Settings
{
    public interface IWorkerRoleSettings
    {
        string StorageConnectionString { get; }
        string WorkerRolePauseTime { get; }
    }
}