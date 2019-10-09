using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Services.Helpers
{
	public class ServiceHelper
	{
		private readonly string _containerName;
		private readonly string _connectionString;

		public ServiceHelper(string connectionString, string containerName)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
		}

		public async Task<CloudBlobContainer> GetCloudBlobContainer()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_containerName);
			await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
			return cloudBlobContainer;
		}

		public byte[] GetImageBytes(Image image)
		{
			using var ms = new MemoryStream();
			image.Save(ms, image.RawFormat);
			return ms.ToArray();
		}
	}
}