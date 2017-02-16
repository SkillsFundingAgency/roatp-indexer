using System.Configuration;

namespace Sfa.Roatp.Indexer.ApplicationServices.Settings
{
    public class ProviderIndexSettings : IIndexSettings<IMaintainProviderIndex>
    {
        public string IndexesAlias => ConfigurationManager.AppSettings["RoatpProviderIndexAlias"];

        public string PauseTime => ConfigurationManager.AppSettings["PauseTime"];
    }
}