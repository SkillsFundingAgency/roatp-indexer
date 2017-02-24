namespace Sfa.Roatp.Indexer.Core.Logging.Models
{
    public class ElasticSearchLogEntry : ILogEntry
    {
        public string Name => "ElasticSearch";

        public int? ReturnCode { get; set; }

        public long? SearchTime { get; set; }

        public double NetworkTime { get; set; }

        public string Url { get; set; }

        public string Body { get; set; }
    }
}
