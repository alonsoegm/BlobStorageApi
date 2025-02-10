using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorageApi.Services.Interfaces;
using System.Text;

namespace AzureBlobStorage.Services;

/// <summary>
/// Provides operations for working with Azure Blob Storage.
/// </summary>
public class BlobService : IBlobService
{
	private readonly BlobServiceClient _blobServiceClient;
	private readonly string _containerName;

	/// <summary>
	/// Initializes a new instance of the <see cref="BlobService"/> class.
	/// </summary>
	/// <param name="configuration">The configuration instance for accessing Blob Storage settings.</param>
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
		await blobClient.UploadAsync(fileStream, overwrite: true);
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
		var blobClient = blobContainerClient.GetBlobClient($"{directoryName.TrimEnd('/')}/placeholder.txt");
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a placeholder for the directory."));
		await blobClient.UploadAsync(stream, overwrite: true);
	}

	public async Task CreateContainer(string containerName)
	{
		await _blobServiceClient.CreateBlobContainerAsync(containerName);
	}

	public async Task<List<BlobItem>> GetBlobList(string containerName)
	{
		List<BlobItem> response = new List<BlobItem>();
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
		await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
		{
			response.Add(blobItem);
		}
		return response;
	}

	public async Task<BlobContainerProperties> GetContainerProperties(string containerName)
	{
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
		return await blobContainerClient.GetPropertiesAsync();
	}

	public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName)
	{
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
		var blobClient = blobContainerClient.GetBlobClient(blobName);
		var properties = await blobClient.GetPropertiesAsync();
		return properties.Value.Metadata;
	}

	public async Task SetBlobMetadataAsync(string blobName, IDictionary<string, string> metadata)
	{
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
		var blobClient = blobContainerClient.GetBlobClient(blobName);
		await blobClient.SetMetadataAsync(metadata);
	}

	public async Task<string> CopyBlobAsync(string sourceBlobName, string destinationBlobName)
	{
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
		var sourceBlobClient = blobContainerClient.GetBlobClient(sourceBlobName);

		var destBlobClient = blobContainerClient.GetBlobClient(destinationBlobName);

		var copyOperation = await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
		
		return destBlobClient.Uri.ToString();
	}

	public async Task<string> CreateBlobSnapshotAsync(string blobName)
	{
		var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
		var blobClient = blobContainerClient.GetBlobClient(blobName);
		var snapshotResponse = await blobClient.CreateSnapshotAsync();
		
		return snapshotResponse.Value.Snapshot;
	}
}

