using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Core.Models;
using Sfa.Roatp.Indexer.Infrastructure.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public class ElasticsearchRoatpDocumentMapper : IElasticsearchRoatpDocumentMapper
    {
        private readonly ILog _logger;
        private readonly IInfrastructureSettings _settings;

        public ElasticsearchRoatpDocumentMapper(ILog logger, IInfrastructureSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public RoatpProviderDocument CreateRoatpProviderDocument(RoatpProvider roatpProvider)
        {
            return new RoatpProviderDocument
            {
                Ukprn = roatpProvider.Ukprn,
                Name = roatpProvider.Name,
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
