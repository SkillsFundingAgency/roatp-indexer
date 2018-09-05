using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Sfa.Roatp.Indexer.ApplicationServices.Settings;

namespace Sfa.Roatp.Indexer.ApplicationServices.Helpers
{
	public class BlobStorageHelper : IBlobStorageHelper
	{
		private readonly IAppServiceSettings _appServiceSettings;

		public BlobStorageHelper(IAppServiceSettings appServiceSettings)
		{
			_appServiceSettings = appServiceSettings;
		}
		
		public CloudBlobContainer GetRoatpBlobContainer()
		{
			return CreateBlobContainer(_appServiceSettings.RoatpBlobContainerReference);
		}
		
		public IEnumerable<string> GetAllBlobs(CloudBlobContainer cloudBlobContainer)
		{
			var blobs = cloudBlobContainer.ListBlobs();

			return (from listBlobItem in blobs where listBlobItem.GetType() == typeof(CloudBlockBlob) select listBlobItem.Uri.ToString()).ToList();
		}

		public IEnumerable<CloudBlockBlob> GetAllBlockBlobs(CloudBlobContainer cloudBlobContainer)
		{
			var blobs = cloudBlobContainer.ListBlobs();

			return (from listBlobItem in blobs where listBlobItem.GetType() == typeof(CloudBlockBlob) select listBlobItem as CloudBlockBlob).ToList();
		}

		private CloudBlobContainer CreateBlobContainer(string containerReference)
		{
			var storageAccount = CloudStorageAccount.Parse(_appServiceSettings.ConnectionString);

			var blobClient = storageAccount.CreateCloudBlobClient();

			var container = blobClient.GetContainerReference(containerReference);

			container.CreateIfNotExists();
			return container;
		}
	}
}
