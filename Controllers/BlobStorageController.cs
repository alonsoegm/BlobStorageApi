using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class BlobController : ControllerBase
{
    private readonly BlobService _blobService;

    public BlobController(BlobService blobService)
    {
        _blobService = blobService;
    }

    // Subir un archivo
    [HttpPost("upload")]
    public async Task<IActionResult> UploadBlob(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var result = await _blobService.UploadBlobAsync(file.FileName, file.OpenReadStream());
        return Ok(new { FileUrl = result });
    }

    // Obtener un archivo
    [HttpGet("get/{fileName}")]
    public async Task<IActionResult> GetBlob(string fileName)
    {
        var fileStream = await _blobService.GetBlobAsync(fileName);
        return File(fileStream, "application/octet-stream", fileName);
    }

    // Eliminar un archivo
    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> DeleteBlob(string fileName)
    {
        var result = await _blobService.DeleteBlobAsync(fileName);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpPost("create-directory")]
    public async Task<IActionResult> CreateDirectory([FromBody] string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
            return BadRequest("Directory name cannot be empty.");

        await _blobService.CreateDirectoryAsync(directoryName);
        return Ok($"Directory '{directoryName}' created successfully.");
    }

}
