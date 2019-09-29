using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Services
{
	public class UploadBlobService
	{
		private readonly string _containerName = "images";
		private readonly string _connectionString;

		public UploadBlobService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task Upload(Image image, string blobName)
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_containerName);
			await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
			var blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			var imageByteArray = GetImageBytes(image);
			await blockBlobReference.UploadFromByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
		}

		private byte[] GetImageBytes(Image image)
		{
			using var ms = new MemoryStream();
			image.Save(ms, image.RawFormat);
			return ms.ToArray();
		}
	}
}