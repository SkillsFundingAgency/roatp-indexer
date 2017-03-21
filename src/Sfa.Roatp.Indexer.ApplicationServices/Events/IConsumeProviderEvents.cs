using System.Threading.Tasks;

namespace Sfa.Roatp.Indexer.ApplicationServices.Events
{
    public interface IConsumeProviderEvents
    {
        Task NewProvider(string ukprn);
    }
}