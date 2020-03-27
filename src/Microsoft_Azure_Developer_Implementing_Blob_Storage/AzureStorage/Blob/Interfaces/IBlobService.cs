using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Interfaces
{
	public interface IBlobService
	{
		Task<CloudBlockBlob> Upload(Image image, string blobName);

		Task<bool> CheckIfBlobExistsAsync(string blobName);

		Task<List<CloudBlobDirectory>> ListBlobDirectories();

		Task<List<CloudBlockBlob>> ListBlobsSegments(string prefix);

		Task DownloadImage(CloudBlockBlob cloudBlockBlob, byte[] data);
	}
}