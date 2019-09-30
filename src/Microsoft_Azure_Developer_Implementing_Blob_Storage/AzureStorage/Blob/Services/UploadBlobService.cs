using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Blob.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Services
{
	public class UploadBlobService : IStorage
	{
		private readonly string _containerName = "images";
		private readonly string _connectionString;

		public UploadBlobService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<CloudBlockBlob> Upload(Image image, string blobName)
		{
			CloudBlobContainer cloudBlobContainer = await GetCloudBlobContainer();
			CloudBlockBlob blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			var imageByteArray = GetImageBytes(image);
			await blockBlobReference.UploadFromByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
			return blockBlobReference;
		}

		private async Task<CloudBlobContainer> GetCloudBlobContainer()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_containerName);
			await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
			return cloudBlobContainer;
		}

		private byte[] GetImageBytes(Image image)
		{
			using var ms = new MemoryStream();
			image.Save(ms, image.RawFormat);
			return ms.ToArray();
		}

		public async Task<bool> CheckIfBlobExistsAsync(string blobName)
		{
			CloudBlobContainer cloudBlobContainer = await GetCloudBlobContainer();
			CloudBlockBlob blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			return await blockBlobReference.ExistsAsync();
		}
	}
}