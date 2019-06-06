using System;
using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.Core.Models
{
    public class RoatpProviderDocument : IEquatable<RoatpProviderDocument>
    {
        public string Ukprn { get; set; }

        public string Name { get; set; }

        public ProviderType ProviderType { get; set; }

        public bool RequiresAgreement => ProviderType == ProviderType.EmployerProvider || ProviderType == ProviderType.MainProvider;

        public bool ContractedForNonLeviedEmployers { get; set; }

        public bool ParentCompanyGuarantee { get; set; }

        public bool NewOrganisationWithoutFinancialTrackRecord { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public DateTime? ApplicationDeterminedDate { get; set; }
        public bool CurrentlyNotStartingNewApprentices { get; set; }

        public bool Equals(RoatpProviderDocument other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Ukprn, other.Ukprn) 
                && ProviderType == other.ProviderType 
                && ContractedForNonLeviedEmployers == other.ContractedForNonLeviedEmployers 
                && ParentCompanyGuarantee == other.ParentCompanyGuarantee 
                && NewOrganisationWithoutFinancialTrackRecord == other.NewOrganisationWithoutFinancialTrackRecord 
                && StartDate.Equals(other.StartDate) 
                && EndDate.Equals(other.EndDate)
                && ApplicationDeterminedDate.Equals(other.ApplicationDeterminedDate)
                && CurrentlyNotStartingNewApprentices == other.CurrentlyNotStartingNewApprentices;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoatpProviderDocument) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Ukprn != null ? Ukprn.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) ProviderType;
                hashCode = (hashCode*397) ^ ContractedForNonLeviedEmployers.GetHashCode();
                hashCode = (hashCode*397) ^ ParentCompanyGuarantee.GetHashCode();
                hashCode = (hashCode*397) ^ NewOrganisationWithoutFinancialTrackRecord.GetHashCode();
                hashCode = (hashCode*397) ^ StartDate.GetHashCode();
                hashCode = (hashCode*397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ ApplicationDeterminedDate.GetHashCode();
                hashCode = (hashCode*397) ^ CurrentlyNotStartingNewApprentices.GetHashCode();
                return hashCode;
            }
        }
    }
}
