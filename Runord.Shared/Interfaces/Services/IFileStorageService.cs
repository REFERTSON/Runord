using Runord.Shared.Base;

namespace Runord.Shared.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<Result<string>> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
        Task<Result<Stream>> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        Task<Result<string>> GetFileUrlAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    }
}