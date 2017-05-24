using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IConsumeProviderEvents
    {
        void NewProvider(RoatpProviderDocument ukprn);
        void ChangedProvider(RoatpProviderDocument next, RoatpProviderDocument last);
    }
}