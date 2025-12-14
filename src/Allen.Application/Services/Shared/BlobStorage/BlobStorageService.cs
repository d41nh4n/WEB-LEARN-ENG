using System.Text.RegularExpressions;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Allen.Application;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }


    public async Task<IEnumerable<string>> GetFileListAsync(string container)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        if (!await containerClient.ExistsAsync())
        {
            return Enumerable.Empty<string>();
        }

        var blobs = new List<string>();
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            blobs.Add(blobItem.Name);
        }
        return blobs;
    }

    public async Task<string> GetFileAsync(string container, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
        }

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> GetFileStreamAsync(string container, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
        }

        return await blobClient.OpenReadAsync();
    }

    public async Task<string> SaveFilesAsync(string container, List<IFormFile> files)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        foreach (var file in files)
        {
            var contentType = GetContentType(file.FileName);
            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };
            var blobClient = containerClient.GetBlobClient(file.FileName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, httpHeaders);
        }

        return $"Uploaded {files.Count} file(s) to container '{container}'.";
    }

    public async Task<string> UploadFileAsync(string containerName, IFormFile file)
    {
        try
        {
            // 🔹 Chuẩn hóa containerName (Azure yêu cầu lowercase, không ký tự lạ)
            containerName = Regex.Replace(containerName.ToLower(), "[^a-z0-9-]", "");

            // 🔹 Tạo container nếu chưa có
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // 🔹 Xử lý tên file: bỏ ký tự lạ, thay space = "-"
            var safeFileName = Path.GetFileName(file.FileName);
            safeFileName = Regex.Replace(safeFileName, @"[^a-zA-Z0-9_.-]", "-");

            // 🔹 Ghép Guid cho duy nhất
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";

            // 🔹 Chuẩn bị header content type
            var contentType = GetContentType(file.FileName);
            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            // 🔹 Upload
            var blobClient = containerClient.GetBlobClient(uniqueFileName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, httpHeaders);

            return blobClient.Uri.ToString();
        }
        catch (RequestFailedException ex)
        {
            throw new Exception($"Lỗi từ Azure Storage: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new Exception($"Không thể đọc file: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi không xác định khi upload file: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string container, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync();
    }
    public async Task<bool> DeleteFileByUrlAsync(string fileUrl)
    {
        // Lấy ra container name và blob name từ URL
        var uri = new Uri(fileUrl);
        // Segments[0] = "/" , Segments[1] = "allen/", Segments[2] = "hat.png"
        var segments = uri.Segments;
        if (segments.Length < 3)
            throw new ArgumentException("URL không hợp lệ");

        var container = segments[1].TrimEnd('/'); // allen
        var blobName = string.Join("", segments.Skip(2)); // hat.png (hoặc path sâu hơn nếu có thư mục con)

        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync();
    }
    public async Task<bool> FileExistsAsync(string container, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.ExistsAsync();
    }

    public async Task<string> GenerateFileAccessUrlAsync(string container, string blobName, TimeSpan validDuration)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{container}'.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validDuration)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var uri = blobClient.GenerateSasUri(sasBuilder);
        return uri.ToString();
    }

    public async Task DeleteAllFilesAsync(string container)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);

        if (!await containerClient.ExistsAsync())
        {
            throw new InvalidOperationException($"Container '{container}' does not exist.");
        }

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }
    }
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            ".mkv" => "video/x-matroska",
            _ => throw new NotSupportedException("Unsupported file extension")
        };
    }
}