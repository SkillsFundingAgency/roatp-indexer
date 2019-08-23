using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices.RoatpClient
{
    public class RoatpMapper: IRoatpMapper
    {
        public List<RoatpProvider> Map(List<RoatpResult> roatpResults)
            {
            return roatpResults.Select(MapSingleRecord).ToList();
            }

        private static RoatpProvider MapSingleRecord(RoatpResult roatpResult)
        {

            ProviderType providerType;
            switch (roatpResult.ProviderType.ToLower())
            {
                case "main provider":
                    providerType = ProviderType.MainProvider;
                    break;
                case "employer provider":
                    providerType = ProviderType.EmployerProvider;
                    break;
                case "supporting provider":
                    providerType = ProviderType.SupportingProvider;
                    break;
                default:
                    providerType = ProviderType.Unknown;
                    break;
            }

            return new RoatpProvider
            {
                Ukprn = roatpResult.Ukprn,
                Name = roatpResult.OrganisationName,
                ProviderType = providerType,
                ContractedForNonLeviedEmployers = roatpResult.ContractedToDeliverToNonLeviedEmployers != null && roatpResult.ContractedToDeliverToNonLeviedEmployers.ToUpper() == "Y",
                ParentCompanyGuarantee = roatpResult.ParentCompanyGuarantee != null && roatpResult.ParentCompanyGuarantee.ToUpper() == "Y",
                NewOrganisationWithoutFinancialTrackRecord = roatpResult.NewOrganisationWithoutFinancialTrackRecord != null & roatpResult.NewOrganisationWithoutFinancialTrackRecord.ToUpper() == "Y",
                StartDate = roatpResult.StartDate,
                EndDate = roatpResult.EndDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null
            };
        }
        }
}
