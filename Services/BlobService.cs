using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    /// <summary>
    /// Constructor to initialize BlobService with configuration.
    /// Retrieves connection string and container name from configuration.
    /// </summary>
    /// <param name="configuration">Configuration object to access settings</param>
    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["AzureBlobStorage:ContainerName"];
    }

    /// <summary>
    /// Uploads a blob to the specified container.
    /// If the container does not exist, it will be created with public access.
    /// </summary>
    /// <param name="fileName">Name of the file to be uploaded</param>
    /// <param name="fileStream">Stream containing the file data</param>
    /// <returns>URL of the uploaded blob</returns>
    public async Task<string> UploadBlobAsync(string fileName, Stream fileStream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        //Creación de un objeto BlobClient
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, true);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Deletes a blob from the specified container if it exists.
    /// </summary>
    /// <param name="fileName">Name of the file to be deleted</param>
    /// <returns>Boolean indicating if the blob was deleted successfully</returns>
    public async Task<bool> DeleteBlobAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //Creación de un objeto BlobClient
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

    /// <summary>
    /// Retrieves a blob as a stream from the specified container.
    /// </summary>
    /// <param name="fileName">Name of the file to be retrieved</param>
    /// <returns>Stream containing the blob data</returns>
    public async Task<Stream> GetBlobAsync(string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //Creación de un objeto BlobClient
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    /// <summary>
    /// Creates a directory in the blob container by uploading a placeholder file.
    /// </summary>
    /// <param name="directoryName">Name of the directory to be created</param>
    public async Task CreateDirectoryAsync(string directoryName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // The directory name must end with a slash "/"
        //Creación de un objeto BlobClient
        var blobClient = blobContainerClient.GetBlobClient($"{directoryName.TrimEnd('/')}/placeholder.txt");

        // Create an empty file as a directory placeholder
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a placeholder for the directory.")))
        {
            await blobClient.UploadAsync(stream, true);
        }
    }

    /// <summary>
    /// Creación de un contenedor
    /// // Create the container and return a container client object
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <returns></returns>
    public async Task CreateContainer(string containerName)
    {
        var blobContainerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);

    }


    /// <summary>
    //Enumerar los blobs de un contenedor
    // List blobs in the container
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <returns>Task<List<BlobItem>></returns>
    public async Task<List<BlobItem>> GetBlobList(string containerName)
    {
        List<BlobItem> respones = new List<BlobItem>();
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var test = blobContainerClient.GetBlobsAsync();

        await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
        {
            respones.Add(blobItem);
        }
        return respones;
    }

    public async Task<BlobContainerProperties> GetContainerProperties(string containerName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return  await blobContainerClient.GetPropertiesAsync();
    }

}

