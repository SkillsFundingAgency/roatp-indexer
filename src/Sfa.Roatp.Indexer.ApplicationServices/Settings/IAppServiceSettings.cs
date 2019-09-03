using System;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public interface IAppServiceSettings
    {
        string GitUsername { get; }

        string GitPassword { get; }

        string VstsRoatpUrl { get; }
        string EnvironmentName { get; }

        string RoatpApiClientBaseUrl { get; }
        string RoatpApiAuthenticationInstance { get; }
        string RoatpApiAuthenticationTenantId { get; }
        string RoatpApiAuthenticationClientId { get; }
        string RoatpApiAuthenticationClientSecret { get; }
        string RoatpApiAuthenticationResourceId { get; }
        string RoatpApiAuthenticationApiBaseAddress { get; }

    }
}