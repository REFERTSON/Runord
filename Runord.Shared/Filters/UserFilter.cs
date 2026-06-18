using Runord.Shared.Base;

namespace Runord.Shared.Filters
{
    public class UserFilter : BaseFilter
    {
        public bool? IsBlocked { get; set; }
        public string? Role { get; set; }
    }
}