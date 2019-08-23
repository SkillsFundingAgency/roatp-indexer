using System.IO;
using System.Net;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.ApplicationServices.RoatpClient;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.NLog.Logger;

namespace Esfa.Roatp.Xslx.IntegrationTests
{
	internal class GetRoatpProvidersIntegrationService : RoatpProvidersXlsxService, IGetRoatpProviders
	{
		public GetRoatpProvidersIntegrationService(IAppServiceSettings appServiceSettings, ILog log, IRoatpApiClient roatpApiClient) 
			: base(appServiceSettings, log,roatpApiClient)
		{
		}

		public override Stream GetFileStream()
		{
			return File.Open(_appServiceSettings.VstsRoatpUrl, FileMode.OpenOrCreate);
		}
	}
}
