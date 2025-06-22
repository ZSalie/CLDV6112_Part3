
public interface IImageRepository
{
    Task<bool> DeleteImageAsync(string fileName);
    Task<Stream> DownloadImageAsync(string fileName);
    Task<IEnumerable<string>> ListImagesAsync();
    Task<string> UploadImageAsync(Stream fileStream, string fileName);
}