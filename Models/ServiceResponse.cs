namespace BlobStorageApi.Models;

/// <summary>
/// Represents a standardized response for service operations.
/// </summary>
/// <typeparam name="T">The type of data returned by the service.</typeparam>
public class ServiceResponse<T>
{
	/// <summary>
	/// Gets or sets the data returned by the service.
	/// </summary>
	public T? Data { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the service operation was successful.
	/// </summary>
	public bool Success { get; set; } = true;

	/// <summary>
	/// Gets or sets a message associated with the service response.
	/// </summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>
	/// Sets an error message and marks the response as unsuccessful.
	/// </summary>
	/// <param name="message">The error message.</param>
	public void SetError(string message)
	{
		Data = default;
		Success = false;
		Message = message;
	}
}
