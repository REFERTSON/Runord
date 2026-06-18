using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Runord.Hub.Configs;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;

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

        public Task<Response<string>> GetDownloadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Response<FileMetadata>> GetFileMetadataAsync(string bucketName, string objectName, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> GetFileUrlAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var args = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(3600);
                var url = await _minioClient.PresignedGetObjectAsync(args);
                return Response<string>.Success(url);
            }
            catch (Exception ex)
            {
                return Response<string>.Failure($"Ошибка получения ссылки: {ex.Message}");
            }
        }

        public Task<Response<string>> GetUploadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            try
            {
                var args = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
                await _minioClient.RemoveObjectAsync(args, cancellationToken);
                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}