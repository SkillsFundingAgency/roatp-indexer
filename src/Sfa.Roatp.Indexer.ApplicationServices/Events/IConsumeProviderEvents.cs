using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IConsumeProviderEvents
    {
        void NewProvider(string ukprn);
    }
}