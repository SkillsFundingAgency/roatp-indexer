using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IConsumeProviderEvents
    {
        void ProcessNewProviderEvents(RoatpProviderDocument ukprn);
        void ProcessChangedProviderEvents(RoatpProviderDocument next, RoatpProviderDocument last);
    }
}