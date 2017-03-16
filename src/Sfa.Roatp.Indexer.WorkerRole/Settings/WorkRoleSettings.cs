using System.Configuration;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Sfa.Roatp.Indexer.WorkerRole.Settings
{
    public class WorkRoleSettings : IWorkerRoleSettings
    {
        private readonly IProvideSettings _settings;

        public WorkRoleSettings(IProvideSettings settingsProvider)
        {
            _settings = settingsProvider;
        }


        public string WorkerRolePauseTime => _settings.GetSetting("WorkerRole.RunFrequencySeconds");
    }
}