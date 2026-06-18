namespace Runord.Shared.Enums
{
    public enum ClusterAction
    {
        Create,
        Update,
        Delete,
        MetricsUpdate,   // изменение метрик (CPU/RAM)
        StatusChange     // изменение статуса (если отдельно)
    }
}