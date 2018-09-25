using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using NServiceBus;
using Sfa.Roatp.Indexer.ApplicationServices.Events;
using Sfa.Roatp.Indexer.WorkerRole.DependencyResolution;
using Sfa.Roatp.Indexer.WorkerRole.Settings;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;

namespace Sfa.Roatp.Indexer.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;
        private ILog _logger;
        private IWorkerRoleSettings _commonSettings;
        private IEndpointInstance _endpoint;

        public override void Run()
        {
            _logger.Debug("Worker role is running");

            while (true)
            {
                try
                {
                    _container.GetInstance<IIndexerJob>().Run();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }

                Thread.Sleep(TimeSpan.FromSeconds(double.Parse(_commonSettings.WorkerRolePauseTime ?? "6000")));
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            _container = IoC.Initialize();
            _logger = _container.GetInstance<ILog>();
            _commonSettings = _container.GetInstance<IWorkerRoleSettings>();

            StartServiceBusEndpoint().GetAwaiter().GetResult();


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            _logger.Debug("Worker role has been started");

            return result;
        }

        public override void OnStop()
        {
            _logger.Debug("Worker role is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            _endpoint.Stop().GetAwaiter().GetResult();

            base.OnStop();

            _logger.Debug("Worker role has stopped");
        }

        private async Task StartServiceBusEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration(_commonSettings.EventsNServiceBusEndpointName)
                  .UseAzureServiceBusTransport(_commonSettings.EventsNServiceBusDevelopmentMode,() => _commonSettings.EventsNServiceBusConnectionString, r => { })
                .UseErrorQueue()
                .UseInstallers()
                //.UseLicense(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseStructureMapBuilder(_container);

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });
        }
    }
}
