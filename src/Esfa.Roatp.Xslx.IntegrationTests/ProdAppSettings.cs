using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Settings;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
    public class ProdAppSettings : AppServiceSettings, IAppServiceSettings
    {
        public ProdAppSettings(IProvideSettings settingsProvider) : base(settingsProvider)
        {
        }

        public override string EnvironmentName => "prod";
    }
}