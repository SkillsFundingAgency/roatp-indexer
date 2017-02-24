using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public class BulkProviderClient
    {
        private readonly List<Task<IBulkResponse>> _tasks;
        private readonly string _indexName;
        private readonly IElasticsearchCustomClient _client;
        private int _count;
        private BulkDescriptor _bulkDescriptor;

        public BulkProviderClient(string indexName, IElasticsearchCustomClient client)
        {
            _bulkDescriptor = CreateBulkDescriptor(indexName);
            _tasks = new List<Task<IBulkResponse>>();
            _indexName = indexName;
            _client = client;
        }

        public BulkDescriptor Create<T>(Func<BulkCreateDescriptor<T>, IBulkCreateOperation<T>> bulkCreateSelector)
            where T : class
        {
            _bulkDescriptor.Create(bulkCreateSelector);
            _count++;
            if (HaveReachedBatchLimit(_count))
            {
                _tasks.Add(_client.BulkAsync(_bulkDescriptor));

                _bulkDescriptor = CreateBulkDescriptor(_indexName);

                _count = 0;
            }

            return _bulkDescriptor;
        }

        public List<Task<IBulkResponse>> GetTasks()
        {
            if (_count > 0)
            {
                _tasks.Add(_client.BulkAsync(_bulkDescriptor));
            }

            return _tasks;
        }

        private static BulkDescriptor CreateBulkDescriptor(string indexName)
        {
            var bulkDescriptor = new BulkDescriptor();
            bulkDescriptor.Index(indexName);

            return bulkDescriptor;
        }

        private static bool HaveReachedBatchLimit(int count)
        {
            const int BatchSize = 4000;

            return count >= BatchSize;
        }
    }
}
