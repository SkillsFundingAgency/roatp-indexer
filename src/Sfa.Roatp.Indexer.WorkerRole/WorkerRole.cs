using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using Sfa.Roatp.Indexer.WorkerRole.DependencyResolution;
using Sfa.Roatp.Indexer.WorkerRole.Settings;
using SFA.DAS.NLog.Logger;
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

        public override void Run()
        {
            Trace.TraceInformation(GetType().FullName + " is running");

            while (true)
            {
                try
                {
                    _container.GetInstance<IIndexerJob>().Run();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception worker role");
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

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation(GetType().FullName + " has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation(GetType().FullName + " is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Sfa.Roatp.Indexer.WorkerRole has stopped");
        }
    }
}
