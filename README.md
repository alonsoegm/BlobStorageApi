# Azure Blob Storage Service API

This project demonstrates a comprehensive implementation of an API to interact with Azure Blob Storage using the official Azure Storage SDK. The solution is designed to be professional, modular, and easily testable, showcasing both basic and advanced interactions with Blob Storage.

## Overview

The API provides endpoints for common and advanced operations, including:
- **Uploading, downloading, and deleting blobs.**
- **Listing blobs within a container.**
- **Creating virtual directories and containers.**
- **Managing blob metadata (get and set).**
- **Copying blobs within the same container.**
- **Creating snapshots of blobs.**

This implementation follows best practices such as dependency injection, interface-driven design, and standardized service responses to facilitate unit testing and maintenance.

## Features

- **Upload Blob:** Upload a file to Azure Blob Storage.
- **Download Blob:** Retrieve a blob as a stream.
- **Delete Blob:** Delete a blob from the container.
- **List Blobs:** List all blobs in a specified container.
- **Get Container Properties:** Retrieve properties of a blob container.
- **Create Directory:** Simulate the creation of a directory by uploading a placeholder file.
- **Create Container:** Create a new blob container.
- **Metadata Management:** Get and set custom metadata for blobs.
- **Copy Blob:** Copy a blob from one location to another within the same container.
- **Create Blob Snapshot:** Create a snapshot of a blob for backup or versioning purposes.

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An active [Azure Storage Account](https://azure.microsoft.com/en-us/services/storage/blobs/)
- Azure Blob Storage credentials. Configure these in your `appsettings.json` file:

    ```json
    {
      "AzureBlobStorage": {
        "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION_STRING",
        "ContainerName": "your-default-container"
      }
    }
    ```

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/azure-blob-storage-api.git
   cd azure-blob-storage-api
   
2. Restore dependencies and build the project:
```bash
dotnet restore
dotnet build
```

4. Run the application:
   ```bash
   dotnet run

5. Access the API:
The API will be available at https://localhost:5001 (or another port as configured). Swagger UI is available at /swagger for interactive API documentation and testing.

Configuration & Dependency Injection
The project uses dependency injection to register all services, ensuring modularity and ease of testing. The main services include:

IBlobService: An interface that defines all blob operations.
BlobService: An implementation of IBlobService that uses the Azure Blob Storage SDK.
BlobServiceAuth: A placeholder service for handling blob service authentication (if needed).
In Program.cs, these services are registered as follows:
```bash
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<BlobServiceAuth>(); // Ensure BlobServiceAuth is implemented
builder.Services.AddControllers();
```
# API Endpoints
## Blob Operations

### Upload Blob

- Endpoint: POST /api/blob/upload
- Description: Uploads a file to Azure Blob Storage.
- Request: Form-data with the file.
- Response: A JSON object containing the URL of the uploaded blob.

### Download Blob

- Endpoint: GET /api/blob/get/{fileName}
- Description: Downloads a blob as a stream.
- Request: The file name as a URL parameter.
- Response: The file stream (application/octet-stream).

### Delete Blob

- Endpoint: DELETE /api/blob/delete/{fileName}
- Description: Deletes the specified blob.
- Request: The file name as a URL parameter.
- Response: A status message indicating success or failure.

### List Blobs

- Endpoint: GET /api/blob/get/{container}/files
- Description: Retrieves a list of blobs within the specified container.
- Request: The container name as a URL parameter.
- Response: A JSON array of blob items.

### Get Container Properties

- Endpoint: GET /api/blob/get/{container}/properties
- Description: Retrieves properties of the specified container.
- Request: The container name as a URL parameter.
- Response: A JSON object with container properties.


## Advanced Operations

### Get Blob Metadata

- Endpoint: GET /api/blob/metadata/{blobName}
- Description: Retrieves metadata for the specified blob.
- Request: The blob name as a URL parameter.
- Response: A JSON dictionary containing metadata key-value pairs.

### Set Blob Metadata

- Endpoint: POST /api/blob/metadata/{blobName}
- Description: Sets custom metadata for the specified blob.
- Request: The blob name as a URL parameter and a metadata dictionary in the request body.
- Response: A status message indicating the operation's success.

### Copy Blob

- Endpoint: POST /api/blob/copy
- Description: Copies a blob from a source to a destination within the same container.
- Request: Query parameters sourceBlobName and destinationBlobName.
- Response: A JSON object containing the URL of the copied blob.

### Create Blob Snapshot

- Endpoint: POST /api/blob/snapshot/{blobName}
- Description: Creates a snapshot of the specified blob.
- Request: The blob name as a URL parameter.
- Response: A JSON object containing the snapshot identifier.

## Testing
The project is designed to be easily testable:

Interface-Based Design: The IBlobService interface allows you to mock blob operations for unit testing.
Standardized Service Responses: The ServiceResponse<T> class ensures consistent API responses, making it easier to assert results in tests.
Unit Testing Frameworks: You can integrate any unit testing framework (such as xUnit, NUnit, or MSTest) to write tests for the controllers and services.
Technologies Used
.NET 8
Azure.Storage.Blobs SDK
ASP.NET Core Web API
Swagger for interactive API documentation
Contributing
Contributions are welcome! If you find any bugs or have feature suggestions, please open an issue or submit a pull request.

License
This project is licensed under the MIT License.
