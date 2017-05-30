using System.Net;
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
            if (string.IsNullOrWhiteSpace(_monitoringSettings.StatusCakeUrl)) return;

            var urls = _monitoringSettings.StatusCakeUrl.Split(';');

            foreach (var url in urls)
            {
                using (var client = new HttpClient())
                {
                    _logger.Debug($"Sending a request to {url}");
                    var task = Task.Run(() => client.GetAsync(url));
                    task.Wait();

                    if (task.Result.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.Warn($"Something failed trying to send a request to StatusCake: {url}");
                    }
                }
            }
        }
    }
}
