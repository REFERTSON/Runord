using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Enums
{
    public enum NotificationType
    {
        /// <summary>Информационное сообщение (задача завершена, файл загружен).</summary>
        Info = 0,

        /// <summary>Предупреждение (задача долго обрабатывается, кластер перегружен).</summary>
        Warning = 1,

        /// <summary>Критическая ошибка (падение кластера, потеря связи).</summary>
        Error = 2,

        /// <summary>Системное уведомление (обновление версии, плановое обслуживание).</summary>
        System = 3
    }
}
