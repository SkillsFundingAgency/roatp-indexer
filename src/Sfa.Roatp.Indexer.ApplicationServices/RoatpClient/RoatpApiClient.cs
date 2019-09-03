using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Models;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices.RoatpClient
{
    public class RoatpApiClient:IRoatpApiClient
    {
        private const string downloadPath = "api/v1/download/roatp-summary";
        private readonly IAppServiceSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILog _log;
        private readonly ITokenService _tokenService;
        private readonly IRoatpMapper _mapper;

        public RoatpApiClient(IAppServiceSettings settings, ILog log, ITokenService tokenService, IRoatpMapper mapper)
        {
            _settings = settings;
            _log = log;
            _tokenService = tokenService;
            _mapper = mapper;
            var baseUrl = _settings.RoatpApiClientBaseUrl;
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        }

        public async Task<List<RoatpProvider>> GetRoatpSummary()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
                _log.Info("Gathering roatp details from API");
                var response = await _httpClient.GetAsync(downloadPath);
                var results = await response.Content.ReadAsAsync<List<RoatpResult>>();
                return _mapper.Map(results);
            }
            catch (Exception e)
            {
                _log.Error(e, $"Error gathering roatp details from API: [{e.Message}]");
                throw;
            }
        }
    }
}

