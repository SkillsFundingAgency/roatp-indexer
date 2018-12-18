using System.IO;
using System.Net;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.NLog.Logger;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
	internal class GetRoatpProvidersIntegrationService : RoatpProvidersXlsxService, IGetRoatpProviders
	{
		public GetRoatpProvidersIntegrationService(IAppServiceSettings appServiceSettings, ILog log) 
			: base(appServiceSettings, log)
		{
		}

		public override Stream GetFileStream()
		{
			return File.Open(_appServiceSettings.VstsRoatpUrl, FileMode.OpenOrCreate);
		}
	}
}
