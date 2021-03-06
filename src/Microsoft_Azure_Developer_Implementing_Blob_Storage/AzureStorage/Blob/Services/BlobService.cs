﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blob.Interfaces;
using Blob.Services.Helpers;
using Microsoft.Azure.Storage.Blob;

namespace Blob.Services
{
	public class BlobService : IBlobService
	{
		private const string ContainerName = @"images";
		private readonly ServiceHelper _serviceHelper;

		public BlobService (string connectionString)
		{
			_serviceHelper = new ServiceHelper(connectionString, ContainerName);
		}

		public async Task<CloudBlockBlob> Upload(Image image, string blobName)
		{
			CloudBlobContainer cloudBlobContainer = await _serviceHelper.GetCloudBlobContainer();
			CloudBlockBlob blockBlobReference = cloudBlobContainer.GetBlockBlobReference(blobName);
			blockBlobReference.Properties.ContentType = @"image/jpeg";
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

		public async Task<List<CloudBlobDirectory>> ListBlobDirectories()
		{
			var cloudBlobContainer = await _serviceHelper.GetCloudBlobContainer();
			BlobResultSegment blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null);
			var items = blobResultSegment.Results.OfType<CloudBlobDirectory>().ToList();
			return items;
		}

		public async Task<List<CloudBlockBlob>> ListBlobsSegments(string prefix)
		{
			var cloudBlobContainer = await _serviceHelper.GetCloudBlobContainer();
			var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.All, null, null, null, null);
			var items = blobResultSegment.Results.OfType<CloudBlockBlob>().ToList();
			return items;
		}

		public async Task DownloadImage(CloudBlockBlob cloudBlockBlob, byte[] data)
		{
			int imageBytes = await cloudBlockBlob.DownloadToByteArrayAsync(data, 0);
		}
	}
}