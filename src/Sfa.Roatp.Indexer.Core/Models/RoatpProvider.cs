using System;

namespace Sfa.Roatp.Indexer.Core.Models
{
    public class RoatpProvider
    {
        public string Ukprn { get; set; }

        public ProviderType ProviderType { get; set; }

        public bool ContractedForNonLeviedEmployers { get; set; }

        public bool ParentCompanyGuarantee { get; set; }

        public bool NewOrganisationWithoutFinancialTrackRecord { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsEqual(RoatpProviderDocument oldRoatpProvider)
        {
            return Ukprn == oldRoatpProvider.Ukprn &&
                   ProviderType == oldRoatpProvider.ProviderType &&
                   ContractedForNonLeviedEmployers == oldRoatpProvider.ContractedForNonLeviedEmployers &&
                   ParentCompanyGuarantee == oldRoatpProvider.ParentCompanyGuarantee &&
                   NewOrganisationWithoutFinancialTrackRecord == oldRoatpProvider.NewOrganisationWithoutFinancialTrackRecord &&
                   StartDate == oldRoatpProvider.StartDate &&
                   EndDate == oldRoatpProvider.EndDate;
        }
    }
}