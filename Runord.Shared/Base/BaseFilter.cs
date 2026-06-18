using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Base
{
    public abstract class BaseFilter
    {
        public string? SearchText { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
