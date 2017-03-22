using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Services;
using SFA.DAS.NLog.Logger;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public class IndexerService<T> : IIndexerService<T>
    {
        private readonly IGenericIndexerHelper<T> _indexerHelper;

        private readonly ILog _log;

        private readonly IIndexSettings<T> _indexSettings;

        private readonly string _name;

        private const string IndexTypeName = "RoATP Provider Index";

        public IndexerService(IIndexSettings<T> indexSettings, IGenericIndexerHelper<T> indexerHelper, ILog log)
        {
            _indexSettings = indexSettings;
            _indexerHelper = indexerHelper;
            _log = log;
            _name = IndexTypeName;
        }

        public async Task CreateScheduledIndex(DateTime scheduledRefreshDateTime)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();

            _log.Info("Checking for updates to ROATP");

            var roatpProviders = _indexerHelper.LoadEntries().ToList();

            var infoHasChanged = _indexerHelper.HasRoatpInfoChanged(roatpProviders);

            if (infoHasChanged)
            {
                _log.Info($"Update to ROATP spreadsheet detected");

                var newIndexName = IndexerHelper.GetIndexNameAndDateExtension(scheduledRefreshDateTime, _indexSettings.IndexesAlias);
                var indexProperlyCreated = _indexerHelper.CreateIndex(newIndexName);

                if (!indexProperlyCreated)
                {
                    throw new Exception($"{_name} index not created properly, exiting...");
                }

                try
                {
                    await _indexerHelper.IndexEntries(newIndexName, roatpProviders).ConfigureAwait(false);

                    CheckIfIndexHasBeenCreated(newIndexName);

                    _indexerHelper.ChangeUnderlyingIndexForAlias(newIndexName);

                    _log.Debug("Swap completed...");

                    _indexerHelper.SendNewProviderEvents(newIndexName);

                    stopwatch.Stop();
                    var properties = new Dictionary<string, object> { { "Alias", _indexSettings.IndexesAlias }, { "ExecutionTime", stopwatch.ElapsedMilliseconds } };
                    _log.Debug($"Created {_name}", properties);
                    _log.Info($"{_name}ing complete.");
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
            }
        }

        private void CheckIfIndexHasBeenCreated(string newIndexName)
        {
            Thread.Sleep(_indexSettings.PauseAfterIndexing);

            for (int i = 0; i < 3; i++)
            {
                if (_indexerHelper.IsIndexCorrectlyCreated(newIndexName))
                {
                    break;
                }

                Thread.Sleep(_indexSettings.PauseAfterIndexing);
            }

            throw new ApplicationException("The new index wasn't found after 3 checks");
        }
    }
}