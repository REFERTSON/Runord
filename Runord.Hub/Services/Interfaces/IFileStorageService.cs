using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;

namespace Runord.Hub.Services.Interfaces
{
    public interface IFileStorageService
    {
        // Метод для получения URL для загрузки файла. Клиент/кластер может использовать этот URL для ЗАГРУЗКИ файла в хранилище.
        Task<Response<string>> GetUploadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken ct = default);

        // Метод для получения URL для скачивания файла. Клиент/кластер может использовать этот URL для СКАЧИВАНИЯ файла из хранилища.
        Task<Response<string>> GetDownloadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken ct = default);

        // Метод для получения метаданных файла, таких как размер, тип и дата последнего изменения.
        Task<Response<FileMetadata>> GetFileMetadataAsync(string bucketName, string objectName, CancellationToken ct = default);

        // Метод для удаления файла из хранилища.
        Task<Response<bool>> DeleteFileAsync(string bucketName, string objectName, CancellationToken ct = default);
    }
}