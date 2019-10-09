using System.Drawing;
using System.Threading.Tasks;
using Blob.Interfaces;
using Blob.Services.Helpers;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Services
{
	public class UploadBlobService : IStorage
	{
		private const string ContainerName = "images";
		private readonly ServiceHelper _serviceHelper;

		public UploadBlobService (string connectionString)
		{
			_serviceHelper = new ServiceHelper(connectionString, ContainerName);
		}

		public async Task<CloudBlockBlob> Upload(Image image, string blobName)
		{
			CloudBlobContainer cloudBlobContainer = await _serviceHelper.GetCloudBlobContainer();
			CloudBlockBlob blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			var imageByteArray = _serviceHelper.GetImageBytes(image);
			await blockBlobReference.UploadFromByteArrayAsync(imageByteArray, 0, imageByteArray.Length);
			return blockBlobReference;
		}

		

		public async Task<bool> CheckIfBlobExistsAsync(string blobName)
		{
			CloudBlobContainer cloudBlobContainer = await _serviceHelper.GetCloudBlobContainer();
			CloudBlockBlob blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			return await blockBlobReference.ExistsAsync();
		}
	}
}