using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:ImageBlobStorage"];
        _containerName = "eventease-images";
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            /*BinaryReader reader = new BinaryReader(fileStream);
            byte[] buffer = new byte[fileStream.Length];
            reader.Read(buffer, 0, buffer.Length);
            BinaryData binaryData = new BinaryData(buffer);*/

            fileStream.Position = 0;
            await blobClient.UploadAsync(fileStream,true);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file: {ex.Message}", ex);
        }
    }

    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException("Image not found in Blob Storage.");

            return await blobClient.OpenReadAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error downloading file: {ex.Message}", ex);
        }
    }

    public async Task<List<string>> ListImagesAsync()
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var imageList = new List<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                imageList.Add(blobItem.Name);
            }

            return imageList;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error listing files: {ex.Message}", ex);
        }
    }
}
