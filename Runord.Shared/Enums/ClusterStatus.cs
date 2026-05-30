using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.Enums
{
    public enum ClusterStatus
    {
        /// <summary>Кластер в сети и доступен для работы</summary>
        Online = 0,

        /// <summary>Кластер не отвечает (heartbeat отсутствует).</summary>
        Offline = 1,
    }
}
