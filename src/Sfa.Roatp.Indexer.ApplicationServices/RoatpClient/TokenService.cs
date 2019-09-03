using System;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices.RoatpClient
{
    public class TokenService: ITokenService
    {
        private readonly IAppServiceSettings _settings;
        private readonly ILog _logger;
        public TokenService(IAppServiceSettings settings, ILog logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public string GetToken()
        {
            {
                if (_settings.RoatpApiAuthenticationApiBaseAddress.Contains("localhost"))
                {
                    return string.Empty;
                }

                var tenantId = _settings.RoatpApiAuthenticationTenantId;
                var clientId = _settings.RoatpApiAuthenticationClientId;
                var appKey = _settings.RoatpApiAuthenticationClientSecret;
                var resourceId = _settings.RoatpApiAuthenticationResourceId;
                var instance = _settings.RoatpApiAuthenticationInstance;

                instance = instance.TrimEnd('/');

                var authority = $"{instance}/{tenantId}";
                try
                {
                    _logger.Debug("Getting client credential");
                    var clientCredential = new ClientCredential(clientId, appKey);
                    _logger.Debug($@"Getting context from authority: {authority}");
                    var context = new AuthenticationContext(authority, true);
                    _logger.Debug($@"acquiring token from resource Id [{resourceId}]");
                    var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;
                    _logger.Debug("access token gathered");
                    return result.AccessToken;
                }
                catch (AggregateException agg)
                {
                    var sb = new StringBuilder();

                    foreach (var inner in agg.InnerExceptions)
                    {
                        sb.AppendLine(inner.ToString());
                    }

                    _logger.Fatal(agg, $@"Fatal aggregate exception error in getting token: [{sb.ToString()}]");

                    throw agg;
                }
                catch (Exception e)
                {
                    _logger.Fatal(e,$@"Fatal error in getting token: [{e.Message}]");

                    
                    throw e;
                }
               

            }
        }
    }
}
