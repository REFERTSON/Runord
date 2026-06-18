using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;

namespace Runord.Shared.Interfaces.Services
{
    public interface IFileClientService
    {
        // Загрузить файл на сервер=
        Task<Response<TaskFileDto>> UploadFileAsync(string bucketName, string objectName, Stream fileStream, CancellationToken ct);

        // Скачать файл с сервера
        Task<Response<Stream>> DownloadFileAsync(string bucketName, string objectName, CancellationToken ct);

        // Получить метаданные
        Task<Response<FileMetadata>> GetFileMetadataAsync(string bucketName, string objectName, CancellationToken ct);

        // Удалить файл
        Task<Response<bool>> DeleteFileAsync(string bucketName, string objectName, CancellationToken ct);
    }
}
