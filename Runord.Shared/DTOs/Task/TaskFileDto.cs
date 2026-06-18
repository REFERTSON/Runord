using Runord.Shared.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Task
{
    // DTO для передачи информации о файлах задачи
    public record TaskFileDto(
        Guid Id,
        string Name,
        FileMetadata Metadata
    ) : BaseDto(Id);

    // Метаданные файла, такие как размер, хэш и дата последнего изменения
    public record FileMetadata(long SizeBytes, string Md5Hash, DateTimeOffset LastModified);
}
