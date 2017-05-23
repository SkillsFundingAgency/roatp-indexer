using System.Net.Http;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices.Monitoring
{
    public class MonitoringService : IMonitoringService
    {
        private readonly IMonitoringSettings _monitoringSettings;
        private readonly ILog _logger;

        public MonitoringService(IMonitoringSettings monitoringSettings, ILog logger)
        {
            _monitoringSettings = monitoringSettings;
            _logger = logger;
        }

        public void SendMonitoringNotification()
        {
            if (string.IsNullOrEmpty(_monitoringSettings.StatusCakeUrl)) return;

            using (var client = new HttpClient())
            {
                var task = Task.Run(() => client.GetAsync(_monitoringSettings.StatusCakeUrl));
                task.Wait();
            }
        }
    }
}
