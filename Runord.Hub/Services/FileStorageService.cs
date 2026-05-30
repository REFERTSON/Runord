using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Runord.Hub.Configs;
using Runord.Shared.Base;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public FileStorageService(IMinioClient minioClient, IOptions<MinioSettings> settings)
        {
            _minioClient = minioClient;
            _settings = settings.Value;
        }

        public async Task<Result<string>> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), cancellationToken);
                if (!bucketExists)
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken);

                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);
                await _minioClient.PutObjectAsync(args, cancellationToken);
                return Result<string>.Success(objectName);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Ошибка загрузки: {ex.Message}");
            }
        }

        public async Task<Result<Stream>> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var memoryStream = new MemoryStream();
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));
                await _minioClient.GetObjectAsync(args, cancellationToken);
                memoryStream.Position = 0;
                return Result<Stream>.Success(memoryStream);
            }
            catch (Exception ex)
            {
                return Result<Stream>.Failure($"Ошибка скачивания: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var args = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
                await _minioClient.RemoveObjectAsync(args, cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ошибка удаления: {ex.Message}");
            }
        }

        public async Task<Result<string>> GetFileUrlAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var args = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(3600);
                var url = await _minioClient.PresignedGetObjectAsync(args);
                return Result<string>.Success(url);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Ошибка получения ссылки: {ex.Message}");
            }
        }
    }
}