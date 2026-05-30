using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Enums
{
    public enum UserRole
    {
        /// <summary>Обычный пользователь – может только смотреть свои задачи.</summary>
        User = 0,

        /// <summary>Полный доступ ко всем ресурсам.</summary>
        Administrator = 1
    }
}
