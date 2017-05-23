using System;
using System.Collections.Generic;

namespace Sfa.Roatp.Indexer.Core.Models
{
    public class RoatpProviderDocument : IEquatable<RoatpProviderDocument>
    {
        public string Ukprn { get; set; }

        public string Name { get; set; }

        public ProviderType ProviderType { get; set; }

        public bool ContractedForNonLeviedEmployers { get; set; }

        public bool ParentCompanyGuarantee { get; set; }

        public bool NewOrganisationWithoutFinancialTrackRecord { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Equals(RoatpProviderDocument other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Ukprn, other.Ukprn) 
                && string.Equals(Name, other.Name) 
                && ProviderType == other.ProviderType 
                && ContractedForNonLeviedEmployers == other.ContractedForNonLeviedEmployers 
                && ParentCompanyGuarantee == other.ParentCompanyGuarantee 
                && NewOrganisationWithoutFinancialTrackRecord == other.NewOrganisationWithoutFinancialTrackRecord 
                && StartDate.Equals(other.StartDate) 
                && EndDate.Equals(other.EndDate);
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
                return hashCode;
            }
        }
    }
}
