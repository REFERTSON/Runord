using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Enums
{
    public enum TaskStatus
    {
        /// <summary>Задача создана, но еще не отправлена в очередь.</summary>
        Created = 0,

        /// <summary>Задача в очереди RabbitMQ, ожидает отправки в кластер.</summary>
        Queued = 1,

        /// <summary>Задача отправлена в Cluster.Server, запущена MPI-сессия.</summary>
        Processing = 2,

        /// <summary>Задача успешно выполнена, результат сохранен в MinIO.</summary>
        Completed = 3,

        /// <summary>Задача прервана из-за ошибки (код возврата MPI ≠ 0).</summary>
        Failed = 4,

        /// <summary>Задача отменена пользователем через UI.</summary>
        Cancelled = 5
    }
}
