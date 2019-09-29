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
			var task = Task.Run(async () => await uploadBlobService.Upload(image, "ben.jpg"));
			task.Wait();
			Console.WriteLine();
		}
	}
}
