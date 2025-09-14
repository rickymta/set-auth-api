using Microsoft.AspNetCore.Http;

namespace Set.Auth.Application.Interfaces;

/// <summary>
/// Interface for storage service
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Upload image to storage and return the file name
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> UploadImageAsync(IFormFile file, string? fileName = null);

    /// <summary>
    /// Get image from storage as stream
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<Stream> GetImageAsync(string fileName);

    /// <summary>
    /// Delete image from storage
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<bool> DeleteImageAsync(string fileName);
}
