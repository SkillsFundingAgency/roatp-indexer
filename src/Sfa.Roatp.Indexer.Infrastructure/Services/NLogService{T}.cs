using System;
using Sfa.Roatp.Indexer.ApplicationServices;
using Sfa.Roatp.Indexer.Infrastructure.Settings;

namespace Sfa.Roatp.Indexer.Infrastructure.Services
{
    public class NLogService<T> : NLogService
    {
        public NLogService(Type loggerType, IInfrastructureSettings settings)
            : base(loggerType, settings)
        {
            if (typeof(T) == typeof(IMaintainProviderIndex))
            {
                ApplicationName = "roatp-regristry-indexer";
            }
        }
    }
}
