namespace Allen.Application;

public interface IBlobStorageService
{
	Task<IEnumerable<string>> GetFileListAsync(string container);
	Task<string> GetFileAsync(string container, string blobName);
	Task<Stream> GetFileStreamAsync(string container, string blobName);
	Task<string> SaveFilesAsync(string container, List<IFormFile> file);
	Task<string> UploadFileAsync(string containerName, IFormFile files);
	Task<bool> FileExistsAsync(string container, string blobName);
	Task<string> GenerateFileAccessUrlAsync(string container, string blobName, TimeSpan validDuration);
	Task<bool> DeleteFileAsync(string container, string blobName);
	Task<bool> DeleteFileByUrlAsync(string fileUrl);
    Task DeleteAllFilesAsync(string container);
}

