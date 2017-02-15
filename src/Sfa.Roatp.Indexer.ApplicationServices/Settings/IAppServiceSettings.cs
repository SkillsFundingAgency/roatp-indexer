using System;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public interface IAppServiceSettings
    {
        string GitUsername { get; }

        string GitPassword { get; }

        string VstsRoatpUrl { get; }

        string EnvironmentName { get; }

        string ConnectionString { get; }

        string QueueName(Type type);
    }
}