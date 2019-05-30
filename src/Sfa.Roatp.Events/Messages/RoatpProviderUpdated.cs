using System;
using Sfa.Roatp.Events.Types;
using SFA.DAS.NServiceBus;

namespace Sfa.Roatp.Events
{
    public class RoatpProviderUpdated : Event
    {
        public MessageType MessageType { get; set; }
        public string Ukprn { get; set; }

        public string Name { get; set; }

        public ProviderType ProviderType { get; set; }

        public bool RequiresAgreement { get; set; }

        public bool ContractedForNonLeviedEmployers { get; set; }

        public bool ParentCompanyGuarantee { get; set; }

        public bool NewOrganisationWithoutFinancialTrackRecord { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public DateTime? RefreshDate { get; set; }

        public bool CurrentlyNotStartingNewApprentices { get; set; }
    }
}
