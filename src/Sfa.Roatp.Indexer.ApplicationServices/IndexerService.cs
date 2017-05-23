﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

        public async Task CheckRoatpAndCreateIndexAndUpdateAlias(DateTime scheduledRefreshDateTime)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();

            SendMonitoringNotification();

            _log.Debug("Checking for updates to ROATP");

            var roatpProviders = _indexerHelper.LoadEntries();
            if (roatpProviders != null)
            {
                var providers = roatpProviders.ToList();

                var infoHasChanged = _indexerHelper.HasRoatpInfoChanged(providers);

                if (infoHasChanged)
                {
                    _log.Debug($"Update to ROATP spreadsheet detected");

                    var newIndexName = IndexerHelper.GetIndexNameAndDateExtension(scheduledRefreshDateTime, _indexSettings.IndexesAlias);
                    var indexProperlyCreated = _indexerHelper.CreateIndex(newIndexName);

                    if (!indexProperlyCreated)
                    {
                        throw new Exception($"{_name} index not created properly, exiting...");
                    }

                    try
                    {
                        await _indexerHelper.IndexEntries(newIndexName, providers).ConfigureAwait(false);

                        CheckIfIndexHasBeenCreated(newIndexName);

                        var stats = _indexerHelper.SendNewProviderEvents(newIndexName);

                        _indexerHelper.ChangeUnderlyingIndexForAlias(newIndexName);

                        _log.Debug("Swap completed...");

                        stopwatch.Stop();
                        var properties = new Dictionary<string, object> {{"Alias", _indexSettings.IndexesAlias}, {"ExecutionTime", stopwatch.ElapsedMilliseconds}};
                        _log.Debug($"Created {_name}", properties);
                        _log.Debug($"{_name}ing complete.");

                        if (stats.TotalCount == 0)
                        {
                            _log.Info("Successfully made changes to existing providers");
                        }
                        else
                        {
                            _log.Info("Successfully updated and added new providers", new Dictionary<string, object> { { "TotalCount", stats.TotalCount } });
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message);
                    }
                }
                else
                {
                    _log.Info("Successfully checked for changes");
                }
            }
        }

        private void SendMonitoringNotification()
        {
            if (string.IsNullOrEmpty(_indexSettings.StatusCakeUrl)) return;

            using (var client = new HttpClient())
            {
                var task = Task.Run(() => client.GetAsync(_indexSettings.StatusCakeUrl));
                task.Wait();
            }
        }

        private void CheckIfIndexHasBeenCreated(string newIndexName)
        {
            Thread.Sleep(TimeSpan.FromSeconds(_indexSettings.PauseAfterIndexing));

            for (int i = 0; i < 3; i++)
            {
                if (_indexerHelper.IsIndexCorrectlyCreated(newIndexName))
                {
                    return;
                }

                Thread.Sleep(TimeSpan.FromSeconds(_indexSettings.PauseAfterIndexing));
            }

            throw new ApplicationException("The new index wasn't found after 3 checks");
        }
    }
}