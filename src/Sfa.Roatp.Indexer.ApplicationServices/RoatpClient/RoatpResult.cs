using System;
using Newtonsoft.Json;

namespace Sfa.Roatp.Indexer.ApplicationServices.RoatpClient
{
    public class RoatpResult
    {
        public string Ukprn { get; set; }

        [JsonProperty("Organisation Name")]
        public string OrganisationName { get; set; }

        [JsonProperty("Provider type")]
        public string ProviderType { get; set; }

        [JsonProperty("Contracted to deliver to non-levied employers")]
        public string ContractedToDeliverToNonLeviedEmployers { get; set; }

        [JsonProperty("Parent company guarantee")]
        public string ParentCompanyGuarantee { get; set; }

        [JsonProperty("New Organisation without financial track record")]
        public string NewOrganisationWithoutFinancialTrackRecord { get; set; }

        [JsonProperty("Start Date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("End Date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("Provider not currently starting new apprentices")]
        public DateTime? ProviderNotCurrentlyStartingNewApprentices { get; set; }

        [JsonProperty("Application Determined Date")]
        public DateTime? ApplicationDeterminedDate { get; set; }

        public bool IsDateValid()
        {
            if (StartDate == null)
            {
                return false;
            }

            if (StartDate?.Date <= DateTime.Today.Date && DateTime.Today.Date <= EndDate)
            {
                return true;
            }

            return StartDate?.Date <= DateTime.Today && EndDate == null;
        }
    }
}
