using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public interface IGetRoatpProviders
    {
        IEnumerable<RoatpProvider> GetRoatpData();
    }
}