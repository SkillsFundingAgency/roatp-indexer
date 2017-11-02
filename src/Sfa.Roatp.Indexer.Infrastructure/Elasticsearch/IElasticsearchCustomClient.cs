using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nest;
using Sfa.Roatp.Indexer.Core.Models;

namespace Sfa.Roatp.Indexer.Infrastructure.Elasticsearch
{
    public interface IElasticsearchCustomClient
    {
        ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, ISearchRequest> selector, [CallerMemberName] string callerName = "")
            where T : class;

        IExistsResponse IndexExists(IndexName index, [CallerMemberName] string callerName = "");

        IDeleteIndexResponse DeleteIndex(IndexName index, [CallerMemberName] string callerName = "");

        IGetMappingResponse GetMapping<T>(Func<GetMappingDescriptor<T>, IGetMappingRequest> selector = null, [CallerMemberName] string callerName = "")
            where T : class;

        IRefreshResponse Refresh(IRefreshRequest request, [CallerMemberName] string callerName = "");

        IRefreshResponse Refresh(Indices indices, Func<RefreshDescriptor, IRefreshRequest> selector = null, [CallerMemberName] string callerName = "");

        IExistsResponse AliasExists(Func<AliasExistsDescriptor, IAliasExistsRequest> selector, [CallerMemberName] string callerName = "");

        IBulkAliasResponse Alias(string aliasName, string indexName, [CallerMemberName] string callerName = "");

        IBulkAliasResponse Alias(IBulkAliasRequest request, [CallerMemberName] string callerName = "");

        IIndicesStatsResponse IndicesStats(Indices indices, Func<IndicesStatsDescriptor, IIndicesStatsRequest> selector = null, [CallerMemberName] string callerName = "");

        IList<string> GetIndicesPointingToAlias(string aliasName, [CallerMemberName] string callerName = "");

        ICreateIndexResponse CreateIndex(IndexName index, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null, [CallerMemberName] string callerName = "");

        Task<IBulkResponse> BulkAsync(IBulkRequest request, [CallerMemberName] string callerName = "");
        void BulkAll(List<RoatpProviderDocument> documents, string indexName);
    }
}