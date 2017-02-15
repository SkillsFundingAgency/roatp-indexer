using System.Configuration;

namespace Sfa.Roatp.Indexer.WorkerRole.Settings
{
    public class WorkRoleSettings : IWorkerRoleSettings
    {
        public string WorkerRolePauseTime => ConfigurationManager.AppSettings["WorkerRolePauseTime"];

        public string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"];
    }
}