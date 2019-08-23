using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.ApplicationServices.RoatpClient
{

        public interface IRoatpApiClient
        {
            Task<List<RoatpProvider>> GetRoatpSummary();
        }
    
}
