using AzureBlobStorage.Services;
using BlobStorageApi.Models;
using BlobStorageApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class BlobController : ControllerBase
{
	// Injected BlobService for handling blob operations
	private readonly IBlobService _blobService;
	// Injected BlobServiceAuth for authentication (if needed)
	private readonly BlobServiceAuth _blobServiceAuth;

    // Constructor: Initializes the controller with the injected services
    public BlobController(IBlobService blobService, BlobServiceAuth blobServiceAuth)
    {
        _blobService = blobService;
        _blobServiceAuth = blobServiceAuth;
    }

    /// <summary>
    /// Uploads a file to Blob Storage.
    /// </summary>
    /// <param name="file">The file to be uploaded.</param>
    /// <returns>A URL of the uploaded file if successful.</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadBlob(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var result = await _blobService.UploadBlobAsync(file.FileName, file.OpenReadStream());
        return Ok(new { FileUrl = result });
    }

    /// <summary>
    /// Retrieves a file from Blob Storage.
    /// </summary>
    /// <param name="fileName">The name of the file to retrieve.</param>
    /// <returns>A file as a Stream if found successfully.</returns>
    [HttpGet("get/{fileName}")]
    public async Task<IActionResult> GetBlob(string fileName)
    {
        var fileStream = await _blobService.GetBlobAsync(fileName);
        return File(fileStream, "application/octet-stream", fileName);
    }

    /// <summary>
    /// Retrieves the list of files within a specific container.
    /// </summary>
    /// <param name="container">The name of the container.</param>
    /// <returns>List of files in the container.</returns>
    [HttpGet("get/{container}/files")]
    public async Task<IActionResult> GetContainerFiles(string container)
    {
        var blobList = await _blobService.GetBlobList(container);
        return Ok(new { files = blobList });
    }

    /// <summary>
    /// Retrieves the properties of the specified container.
    /// </summary>
    /// <param name="container">The name of the container.</param>
    /// <returns>Properties of the container.</returns>
    [HttpGet("get/{container}/properties")]
    public async Task<IActionResult> GetContainerProperties(string container)
    {
        var blobProperties = await _blobService.GetContainerProperties(container);
        return Ok(new { files = blobProperties });
    }

    /// <summary>
    /// Deletes a file from Blob Storage.
    /// </summary>
    /// <param name="fileName">The name of the file to delete.</param>
    /// <returns>An Ok result if deleted successfully, NotFound if the file is not found.</returns>
    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> DeleteBlob(string fileName)
    {
        var result = await _blobService.DeleteBlobAsync(fileName);
        if (result)
            return Ok();
        return NotFound();
    }

    /// <summary>
    /// Creates a new directory within Blob Storage.
    /// </summary>
    /// <param name="directoryName">The name of the directory to create.</param>
    /// <returns>Message indicating the directory was created successfully.</returns>
    [HttpPost("create-directory")]
    public async Task<IActionResult> CreateDirectory([FromBody] string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
            return BadRequest("Directory name cannot be empty.");

        await _blobService.CreateDirectoryAsync(directoryName);
        return Ok($"Directory '{directoryName}' created successfully.");
    }

    /// <summary>
    /// Creates a new container within Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container to create.</param>
    /// <returns>Message indicating the container was created successfully.</returns>
    [HttpPost("create-container")]
    public async Task<IActionResult> CreateContainer([FromBody] string containerName)
    {
        if (string.IsNullOrWhiteSpace(containerName))
            return BadRequest("Container name cannot be empty.");

        await _blobService.CreateContainer(containerName);
        return Ok($"Container '{containerName}' created successfully.");
    }

	/// <summary>
	/// Retrieves metadata for the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob for which metadata will be retrieved.</param>
	/// <returns>A ServiceResponse containing a dictionary of metadata key-value pairs.</returns>
	[HttpGet("metadata/{blobName}")]
	public async Task<IActionResult> GetBlobMetadata(string blobName)
	{
		try
		{
			var metadata = await _blobService.GetBlobMetadataAsync(blobName);
			return Ok(new ServiceResponse<IDictionary<string, string>>
			{
				Data = metadata,
				Message = "Metadata retrieved successfully."
			});
		}
		catch (Exception ex)
		{
			var response = new ServiceResponse<IDictionary<string, string>>();
			response.SetError($"Error retrieving metadata: {ex.Message}");
			return StatusCode(500, response);
		}
	}

	/// <summary>
	/// Sets custom metadata for the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob for which metadata will be set.</param>
	/// <param name="metadata">A dictionary containing metadata key-value pairs.</param>
	/// <returns>A ServiceResponse indicating whether the metadata was set successfully.</returns>
	[HttpPost("metadata/{blobName}")]
	public async Task<IActionResult> SetBlobMetadata(string blobName, [FromBody] IDictionary<string, string> metadata)
	{
		try
		{
			await _blobService.SetBlobMetadataAsync(blobName, metadata);
			return Ok(new ServiceResponse<string>
			{
				Data = blobName,
				Message = "Metadata set successfully."
			});
		}
		catch (Exception ex)
		{
			var response = new ServiceResponse<string>();
			response.SetError($"Error setting metadata: {ex.Message}");
			return StatusCode(500, response);
		}
	}

	/// <summary>
	/// Copies a blob from a source location to a destination within the same container.
	/// </summary>
	/// <param name="sourceBlobName">The name of the source blob.</param>
	/// <param name="destinationBlobName">The name of the destination blob.</param>
	/// <returns>A ServiceResponse containing the URL of the copied blob.</returns>
	[HttpPost("copy")]
	public async Task<IActionResult> CopyBlob([FromQuery] string sourceBlobName, [FromQuery] string destinationBlobName)
	{
		try
		{
			var destinationUrl = await _blobService.CopyBlobAsync(sourceBlobName, destinationBlobName);
			return Ok(new ServiceResponse<string>
			{
				Data = destinationUrl,
				Message = "Blob copied successfully."
			});
		}
		catch (Exception ex)
		{
			var response = new ServiceResponse<string>();
			response.SetError($"Error copying blob: {ex.Message}");
			return StatusCode(500, response);
		}
	}

	/// <summary>
	/// Creates a snapshot of the specified blob.
	/// </summary>
	/// <param name="blobName">The name of the blob to snapshot.</param>
	/// <returns>A ServiceResponse containing the snapshot identifier.</returns>
	[HttpPost("snapshot/{blobName}")]
	public async Task<IActionResult> CreateBlobSnapshot(string blobName)
	{
		try
		{
			var snapshotId = await _blobService.CreateBlobSnapshotAsync(blobName);
			return Ok(new ServiceResponse<string>
			{
				Data = snapshotId,
				Message = "Snapshot created successfully."
			});
		}
		catch (Exception ex)
		{
			var response = new ServiceResponse<string>();
			response.SetError($"Error creating snapshot: {ex.Message}");
			return StatusCode(500, response);
		}
	}
}
