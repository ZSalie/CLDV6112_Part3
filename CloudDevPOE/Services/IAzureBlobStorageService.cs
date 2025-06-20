
public interface IAzureBlobStorageService
{
    Task<bool> DeleteImageAsync(string fileName);
    Task<Stream> DownloadImageAsync(string fileName);
    Task<List<string>> ListImagesAsync();
    Task<string> UploadImageAsync(Stream fileStream, string fileName);
}