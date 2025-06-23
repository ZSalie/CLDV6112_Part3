using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using NuGet.Protocol.Plugins;

public class LocalImageStorageService : IImageRepository
{
    private readonly string _imageDirectory;

    public LocalImageStorageService(IWebHostEnvironment env)
    {
        _imageDirectory = Path.Combine(env.WebRootPath, "image-data");
        if (!Directory.Exists(_imageDirectory))
        {
            Directory.CreateDirectory(_imageDirectory);
        }
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(_imageDirectory, fileName);
        fileStream.Seek(0, SeekOrigin.Begin);
        using (var dataStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(dataStream);
        }
        return fileName;
    }

    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        var filePath = Path.Combine(_imageDirectory, fileName);
        if (!File.Exists(filePath))
            throw new Exception("File not found");

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> DeleteImageAsync(string fileName)
    {
        var filePath = Path.Combine(_imageDirectory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<IEnumerable<string>> ListImagesAsync()
    {
        if (!Directory.Exists(_imageDirectory))
            return Task.FromResult<IEnumerable<string>>(new List<string>());
        var files = Directory.GetFiles(_imageDirectory);
        var fileNames = new List<string>();
        foreach (var file in files)
        {
            fileNames.Add(Path.GetFileName(file));
        }
        return Task.FromResult<IEnumerable<string>>(fileNames);
    }
}
