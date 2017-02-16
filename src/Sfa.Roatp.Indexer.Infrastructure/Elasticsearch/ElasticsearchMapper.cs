using System;
using Sfa.Roatp.Indexer.ApplicationServices.Models;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public class ElasticsearchMapper : IElasticsearchMapper
    {
        private readonly ILog _logger;
        private readonly IInfrastructureSettings _settings;

        public ElasticsearchMapper(ILog logger, IInfrastructureSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public RoatpProviderDocument CreateRoatpProviderDocument(RoatpProvider roatpProvider)
        {
            return new RoatpProviderDocument
            {
                Ukprn = roatpProvider.Ukprn,
                ContractedForNonLeviedEmployers = roatpProvider.ContractedForNonLeviedEmployers,
                StartDate = roatpProvider.StartDate,
                EndDate = roatpProvider.EndDate,
                NewOrganisationWithoutFinancialTrackRecord = roatpProvider.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = roatpProvider.ParentCompanyGuarantee,
                ProviderType = roatpProvider.ProviderType
            };
        }
    }
}
