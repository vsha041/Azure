using System;
using System.Drawing;
using System.Threading.Tasks;
using Blob.Services;
using Blob.Settings;

namespace Blob
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Upload Blob
			var image = Image.FromFile(@"Images\ben.jpg");
			var uploadBlobService = new UploadBlobService(AppSettings.ConnectionString);
			var task = Task.Run(async () =>
			{
				var blobName = "2019\\ben.jpg";
				var doesBlobExists = await uploadBlobService.CheckIfBlobExistsAsync(blobName);
				if (!doesBlobExists)
				{
					var cloudBlockBlob = await uploadBlobService.Upload(image, blobName);
					Console.WriteLine($"{cloudBlockBlob.Name} - {cloudBlockBlob.Uri.AbsoluteUri}");
				}
				else
				{
					Console.WriteLine($"Blob with name {blobName} already exists.");
				}
			});

			task.Wait();
		}
	}
}