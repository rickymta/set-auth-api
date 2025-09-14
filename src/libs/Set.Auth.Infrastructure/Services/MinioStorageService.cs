using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Set.Auth.Application.Interfaces;

namespace Set.Auth.Infrastructure.Services;

/// <summary>
/// Minio storage service implementation
/// </summary>
/// <remarks>
/// Constructor for MinioStorageService
/// </remarks>
/// <param name="minioClient"></param>
/// <param name="configuration"></param>
/// <param name="logger"></param>
public class MinioStorageService(
    IMinioClient minioClient,
    IConfiguration configuration,
    ILogger<MinioStorageService> logger) : IStorageService
{
    /// <summary>
    /// Bucket name from configuration
    /// </summary>
    private readonly string _bucketName = configuration["MinIO:BucketName"] ?? "images";

    /// <inheritdoc />
    public async Task<string> UploadImageAsync(IFormFile file, string? fileName = null)
    {
        try
        {
            // Ensure bucket exists
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var bucketExists = await minioClient.BucketExistsAsync(bucketExistsArgs);

            if (!bucketExists)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                await minioClient.MakeBucketAsync(makeBucketArgs);
            }

            // Generate unique filename if not provided
            fileName ??= $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Upload file
            using var stream = file.OpenReadStream();

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(file.ContentType);

            await minioClient.PutObjectAsync(putObjectArgs);

            return fileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading image to MinIO");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Stream> GetImageAsync(string fileName)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await minioClient.GetObjectAsync(getObjectArgs);
            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving image from MinIO");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);

            await minioClient.RemoveObjectAsync(removeObjectArgs);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting image from MinIO");
            return false;
        }
    }
}
