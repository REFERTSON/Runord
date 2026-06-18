using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Filters
{
    public class ClusterFilter : BaseFilter
    {
        public ClusterStatus? Status { get; set; }
    }
}