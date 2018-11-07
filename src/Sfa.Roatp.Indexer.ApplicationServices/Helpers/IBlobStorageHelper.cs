using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sfa.Roatp.Indexer.ApplicationServices.Helpers
{
	public interface IBlobStorageHelper
	{
		IEnumerable<string> GetAllBlobs(CloudBlobContainer cloudBlobContainer);
		IEnumerable<CloudBlockBlob> GetAllBlockBlobs(CloudBlobContainer cloudBlobContainer);
		CloudBlobContainer GetRoatpBlobContainer();
	}
}