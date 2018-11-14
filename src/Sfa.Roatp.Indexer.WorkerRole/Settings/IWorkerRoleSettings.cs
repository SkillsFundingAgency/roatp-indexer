namespace Sfa.Roatp.Indexer.WorkerRole.Settings
{
    public interface IWorkerRoleSettings
    {
        string WorkerRolePauseTime { get; }
        string EventsNServiceBusConnectionString { get; }
        string EventsNServiceBusEndpointName { get; }
        bool EventsNServiceBusDevelopmentMode { get; }
    }
}