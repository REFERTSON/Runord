using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.Shared.Enums
{
    public enum TaskItemType
    {
        Sorting,
        MatrixMultiplication,
        PiCalculation,
        KMeansClustering,
        GraphAlgorithm,
        DataCompression,
        FFT,
        Encryption,
    }

    public static class TaskItemTypeDescriptions
    {
        public static readonly IReadOnlyDictionary<TaskItemType, string> Descriptions = new Dictionary<TaskItemType, string>
        {
            [TaskItemType.Sorting] = "Сортировка",
            [TaskItemType.MatrixMultiplication] = "Умножение матриц",
            [TaskItemType.PiCalculation] = "Вычисление Пи",
            [TaskItemType.KMeansClustering] = "Кластеризация",
            [TaskItemType.GraphAlgorithm] = "Графовые алгоритмы",
            [TaskItemType.DataCompression] = "Сжатие данных",
            [TaskItemType.FFT] = "Быстрое преобразование Фурье",
            [TaskItemType.Encryption] = "Шифрование/хеширование данных"
        };

        public static string GetDescription(TaskItemType taskType)
            => Descriptions.TryGetValue(taskType, out var desc) ? desc : taskType.ToString();
    }
}
