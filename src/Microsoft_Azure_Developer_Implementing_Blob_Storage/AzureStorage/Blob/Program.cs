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
			Console.WriteLine("-----------------------------------------------");
			var image = Image.FromFile(@"Images\ben.jpg");
			var blobService = new BlobService(AppSettings.ConnectionString);
			var task = Task.Run(async () =>
			{
				var blobName = "2019\\ben.jpg";
				var doesBlobExists = await blobService.CheckIfBlobExistsAsync(blobName);
				if (!doesBlobExists)
				{
					var cloudBlockBlob = await blobService.Upload(image, blobName);
					Console.WriteLine($"{cloudBlockBlob.Name} - {cloudBlockBlob.Uri.AbsoluteUri}");
				}
				else
				{
					Console.WriteLine($"Blob with name {blobName} already exists.");
				}
			});

			task.Wait();

			// List blob directory
			Console.WriteLine("-----------------------------------------------");
			var listBlobTask = Task.Run(async () =>
			{
				var cloudBlockDirectories = await blobService.ListBlobDirectories();
				foreach (var cloudBlockDirectory in cloudBlockDirectories)
				{
					Console.WriteLine($"{cloudBlockDirectory.Uri}");
				}
			});

			listBlobTask.Wait();

			// List Blob Segments
			Console.WriteLine("-----------------------------------------------");
			var listBlobSegmentTask = Task.Run(async () =>
			{
				var cloudBlockBlobSegments = await blobService.ListBlobsSegments("2019/Ch");
				foreach (var cloudBlockSegment in cloudBlockBlobSegments)
				{
					Console.WriteLine($"{cloudBlockSegment.Name} - {cloudBlockSegment.Uri}");
				}
			});

			listBlobSegmentTask.Wait();
		}
	}
}