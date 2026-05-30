using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Enums
{
    public enum TaskPriority
    {
        /// <summary>Низкий приоритет – выполняется в фоне, после других задач.</summary>
        Low = 0,

        /// <summary>Средний приоритет – стандартный уровень.</summary>
        Medium = 1,

        /// <summary>Высокий приоритет – задачи ставятся в начало очереди.</summary>
        High = 2,
    }
}
