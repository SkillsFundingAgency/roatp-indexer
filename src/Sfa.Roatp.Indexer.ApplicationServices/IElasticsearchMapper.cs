using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IElasticsearchRoatpDocumentMapper
    {
        RoatpProviderDocument CreateRoatpProviderDocument(RoatpProvider roatpProvider);
    }
}