using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _blobServiceClient = new BlobServiceClient(connectionString);
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

        // El nombre del directorio debe terminar con una barra "/"
        var blobClient = blobContainerClient.GetBlobClient($"{directoryName.TrimEnd('/')}/placeholder.txt");

        // Creamos un archivo vacío como marcador de directorio
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a placeholder for the directory.")))
        {
            await blobClient.UploadAsync(stream, true);
        }
    }

}
