using Azure.Storage.Blobs.Models;

namespace BlobStorageApi.Services.Interfaces;

/// <summary>
/// Defines the contract for Blob Storage operations.
/// </summary>
public interface IBlobService
{
	/// <summary>
	/// Uploads a file to Azure Blob Storage.
	/// </summary>
	/// <param name="fileName">The name of the file to upload.</param>
	/// <param name="fileStream">The stream containing the file data.</param>
	/// <returns>The URL of the uploaded blob.</returns>
	Task<string> UploadBlobAsync(string fileName, Stream fileStream);

	/// <summary>
	/// Deletes a blob from Azure Blob Storage.
	/// </summary>
	/// <param name="fileName">The name of the blob to delete.</param>
	/// <returns>A boolean indicating whether the blob was deleted.</returns>
	Task<bool> DeleteBlobAsync(string fileName);

	/// <summary>
	/// Retrieves a blob from Azure Blob Storage as a stream.
	/// </summary>
	/// <param name="fileName">The name of the blob to retrieve.</param>
	/// <returns>A stream containing the blob's content.</returns>
	Task<Stream> GetBlobAsync(string fileName);

	/// <summary>
	/// Creates a virtual directory in Azure Blob Storage by uploading a placeholder file.
	/// </summary>
	/// <param name="directoryName">The name of the directory to create.</param>
	Task CreateDirectoryAsync(string directoryName);

	/// <summary>
	/// Creates a new blob container.
	/// </summary>
	/// <param name="containerName">The name of the container to create.</param>
	Task CreateContainer(string containerName);

	/// <summary>
	/// Retrieves a list of blobs from the specified container.
	/// </summary>
	/// <param name="containerName">The name of the container.</param>
	/// <returns>A list of blobs in the container.</returns>
	Task<List<BlobItem>> GetBlobList(string containerName);

	/// <summary>
	/// Retrieves the properties of the specified blob container.
	/// </summary>
	/// <param name="containerName">The name of the container.</param>
	/// <returns>The properties of the container.</returns>
	Task<BlobContainerProperties> GetContainerProperties(string containerName);

	/// <summary>
	/// Retrieves the metadata of the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob.</param>
	/// <returns>A dictionary containing the blob's metadata.</returns>
	Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName);

	/// <summary>
	/// Sets custom metadata for the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob.</param>
	/// <param name="metadata">A dictionary with the metadata to set.</param>
	Task SetBlobMetadataAsync(string blobName, IDictionary<string, string> metadata);

	/// <summary>
	/// Copies a blob from one location to another within the same storage account.
	/// </summary>
	/// <param name="sourceBlobName">The name of the source blob.</param>
	/// <param name="destinationBlobName">The name of the destination blob.</param>
	/// <returns>The URL of the newly copied blob.</returns>
	Task<string> CopyBlobAsync(string sourceBlobName, string destinationBlobName);

	/// <summary>
	/// Creates a snapshot of the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob to snapshot.</param>
	/// <returns>The snapshot identifier (or URI) of the created snapshot.</returns>
	Task<string> CreateBlobSnapshotAsync(string blobName);
}