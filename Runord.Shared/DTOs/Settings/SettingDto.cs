using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Settings
{
    public record SettingDto<T>(string Key, T Value);
}
