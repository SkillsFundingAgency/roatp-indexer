namespace Sfa.Roatp.Indexer.Infrastructure.Settings
{
    public interface IElasticsearchSettings
    {
        string[] StopWords { get; }

        string[] StopWordsExtended { get; }

        string[] Synonyms { get; }

        string ApprenticeshipIndexShards { get; }

        string ApprenticeshipIndexReplicas { get; }

        string RoatpProviderIndexShards { get; }

        string RoatpProviderIndexReplicas { get; }

        string LarsIndexShards { get; }

        string LarsIndexReplicas { get; }
    }
}