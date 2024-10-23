using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

public class BlobServiceAuth
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobServiceAuth(IConfiguration configuration)
    {
        var accountName = configuration["AzureBlobStorage:AccountName"];
        _blobServiceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());
        _containerName = configuration["AzureBlobStorage:ContainerName"];
    }

    public async Task<string> UploadBlobAsync(string fileName, Stream fileStream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, true);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteBlobAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Stream> GetBlobAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    public async Task CreateDirectoryAsync(string directoryName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // The directory name must end with a slash "/"
        var blobClient = blobContainerClient.GetBlobClient($"{directoryName.TrimEnd('/')}/placeholder.txt");

        // Create an empty file as a placeholder for the directory
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a placeholder for the directory.")))
        {
            await blobClient.UploadAsync(stream, true);
        }
    }
}
