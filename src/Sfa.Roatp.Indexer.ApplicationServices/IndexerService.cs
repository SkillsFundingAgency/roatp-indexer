﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;
using Sfa.Roatp.Indexer.Core.Services;
using Sfa.Roatp.Registry.Core.Logging;

namespace Sfa.Roatp.Indexer.ApplicationServices
{
    public class IndexerService<T> : IIndexerService<T>
    {
        private readonly IGenericIndexerHelper<T> _indexerHelper;

        private readonly ILog _log;

        private readonly IIndexSettings<T> _indexSettings;

        private readonly string _name;

        public IndexerService(IIndexSettings<T> indexSettings, IGenericIndexerHelper<T> indexerHelper, ILog log)
        {
            _indexSettings = indexSettings;
            _indexerHelper = indexerHelper;
            _log = log;
            _name = GetIndexTypeName(typeof(T));
        }

        public async Task CreateScheduledIndex(DateTime scheduledRefreshDateTime)
        {
            _log.Info($"Creating new scheduled {_name}");

            Stopwatch stopwatch = Stopwatch.StartNew();

            var newIndexName = IndexerHelper.GetIndexNameAndDateExtension(scheduledRefreshDateTime, _indexSettings.IndexesAlias);
            var indexProperlyCreated = _indexerHelper.CreateIndex(newIndexName);

            if (!indexProperlyCreated)
            {
                throw new Exception($"{_name} index not created properly, exiting...");
            }

            _log.Info($"Indexing documents for {_name}.");

            try
            {
                await _indexerHelper.IndexEntries(newIndexName).ConfigureAwait(false);

                PauseWhileIndexingIsBeingRun();

                var indexHasBeenCreated = _indexerHelper.IsIndexCorrectlyCreated(newIndexName);
                if (indexHasBeenCreated)
                {
                    _indexerHelper.ChangeUnderlyingIndexForAlias(newIndexName);

                    _log.Debug("Swap completed...");

                    _indexerHelper.DeleteOldIndexes(scheduledRefreshDateTime);
                }

                stopwatch.Stop();
                var properties = new Dictionary<string, object> { { "Alias", _indexSettings.IndexesAlias }, { "ExecutionTime", stopwatch.ElapsedMilliseconds }, { "IndexCorrectlyCreated", indexHasBeenCreated } };
                _log.Debug($"Created {_name}", properties);
                _log.Info($"{_name}ing complete.");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error indexing");
            }
        }

        private void PauseWhileIndexingIsBeingRun()
        {
            var time = _indexSettings.PauseTime;
            Thread.Sleep(int.Parse(time));
        }

        private string GetIndexTypeName(Type type)
        {
            return type == typeof(IMaintainProviderIndex) ? "RoATP Provider Index" : "Unknown";
        }
    }
}