using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Enums;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace Runord.Client.ViewModels
{
    public partial class ClusterListViewModel : ViewModelBase
    {
        private readonly IClusterService _clusterService;

        [ObservableProperty] private ObservableCollection<NodeItemViewModel> _nodesList = new();
        [ObservableProperty] private bool _isAddDialogVisible;
        [ObservableProperty] private string _dialogTitle = "Добавление кластера";
        [ObservableProperty] private string _newNodeName = "";
        [ObservableProperty] private string _newNodeIp = "";
        private NodeItemViewModel? _editingNode;

        [ObservableProperty] private bool _isFilterDialogVisible;
        [ObservableProperty] private string _filterSearchText = "";
        [ObservableProperty] private bool _isFilterAll = true;
        [ObservableProperty] private bool _isFilterOnline;
        [ObservableProperty] private bool _isFilterOffline;
        private ClusterFilter CurrentFilter { get; set; } = new();

        [ObservableProperty] private string _bannerHeader = "";
        [ObservableProperty] private string _bannerMessage = "";
        private readonly Random _random = new();
        private readonly (string Header, string Message)[] _bannerVariants = new[]
        {
            ("Хаб скучает в одиночестве... ( •_•)", "Список кластеров пуст. Без активных кластеров Хабу просто не на чем запускать параллельные задачи. Добавьте оборудование, чтобы вдохнуть в него жизнь."),
            ("Кластеры ушли на бесконечный перерыв и все ¯\\_(ツ)_/¯", "В данный момент нет ни одного активного кластера. Нажмите кнопку добавления сверху, чтобы запустить параллельную обработку задач."),
            ("Провода на месте, а вычислять некому  (o_O)", "Хаб готов к работе, но пустует. Подключите хотя бы один кластер, чтобы клиенты могли отправлять свои вычислительные задачи."),
            ("Локальный кризис мощностей  (;-;)", "Все кластеры куда-то испарились. Добавьте новые кластеры, чтобы вернуть распределенную систему в рабочее состояние."),
            ("Кластеры объявили забастовку  (╯°□°）╯︵ ┻━┻", "Доступных серверов не обнаружено. Самое время нажать «Добавить кластер» и вернуть систему к жизни."),
            ("Здесь могла быть ваша вычислительная мощность  (¬_¬)", "Но пока тут абсолютно пусто. Добавьте хотя бы один кластер, чтобы Хаб перестал работать вхолостую."),
            ("Тишина в эфире...  [ ._. ]", "Ни один кластер не вышел на связь. Разверните новые кластеры сверху справа, чтобы запустить параллельную обработку."),
            ("В поисках потерянных серверов  (ಠ_ಠ)", "Текущий список кластеров пуст. Без подключенных кластеров задачи клиентов так и останутся висеть в очереди."),
            ("Хаб работает на честном слове  (́ ʘ 凸 ʘ ̀)", "Потому что реальных кластеров в списке нет. Срочно добавьте кластеры, пока всё окончательно не замерло."),
            ("Похоже, кто-то выдернул кабель  (>_<)", "Доступные кластеры отсутствуют. Добавьте кластеры через верхнюю панель, чтобы распределить вычислительную нагрузку.")
        };

        public ClusterListViewModel(IClusterService clusterService)
        {
            _clusterService = clusterService;
            LoadClusters();
            PickRandomBanner();
        }

        private async void LoadClusters()
        {
            await ExecuteAsync(async () =>
            {
                var result = await _clusterService.GetClustersAsync(null, CancellationToken.None);
                if (result.IsSuccess && result.Data != null)
                {
                    NodesList.Clear();
                    foreach (var dto in result.Data)
                        NodesList.Add(new NodeItemViewModel(dto));
                    ApplyFilterInternal();
                }
            });
        }

        private void PickRandomBanner()
        {
            var idx = _random.Next(_bannerVariants.Length);
            BannerHeader = _bannerVariants[idx].Header;
            BannerMessage = _bannerVariants[idx].Message;
        }

        private void ApplyFilterInternal()
        {
            var view = CollectionViewSource.GetDefaultView(NodesList);
            view.Filter = obj =>
            {
                if (obj is not NodeItemViewModel node) return false;
                if (!string.IsNullOrWhiteSpace(CurrentFilter.SearchText))
                {
                    if (!node.Name.Contains(CurrentFilter.SearchText, StringComparison.OrdinalIgnoreCase) &&
                        !node.Ip.Contains(CurrentFilter.SearchText, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                if (CurrentFilter.Status.HasValue && node.Status != CurrentFilter.Status.Value)
                    return false;
                return true;
            };
            view.Refresh();
            OnPropertyChanged(nameof(IsListEmpty));
            if (view.IsEmpty && NodesList.Count > 0)
            {
                BannerHeader = "Ничего не найдено";
                BannerMessage = "Измените параметры фильтра";
            }
            else if (NodesList.Count == 0) PickRandomBanner();
        }

        public int TotalCount => NodesList.Count;
        public int OnlineCount => NodesList.Count(x => x.Status == ClusterStatus.Online);
        public int OfflineCount => NodesList.Count(x => x.Status == ClusterStatus.Offline);
        public bool IsListEmpty => NodesList.Count == 0;

        [RelayCommand]
        private void OpenFilterDialog()
        {
            FilterSearchText = CurrentFilter.SearchText ?? "";
            IsFilterAll = CurrentFilter.Status == null;
            IsFilterOnline = CurrentFilter.Status == ClusterStatus.Online;
            IsFilterOffline = CurrentFilter.Status == ClusterStatus.Offline;
            IsFilterDialogVisible = true;
        }
        [RelayCommand] private void CloseFilterDialog() => IsFilterDialogVisible = false;
        [RelayCommand]
        private void ApplyFilter()
        {
            CurrentFilter.SearchText = FilterSearchText;
            if (IsFilterAll) CurrentFilter.Status = null;
            else if (IsFilterOnline) CurrentFilter.Status = ClusterStatus.Online;
            else CurrentFilter.Status = ClusterStatus.Offline;
            ApplyFilterInternal();
            IsFilterDialogVisible = false;
        }
        [RelayCommand]
        private void ResetFilter()
        {
            CurrentFilter = new ClusterFilter();
            FilterSearchText = "";
            IsFilterAll = true;
            ApplyFilterInternal();
            IsFilterDialogVisible = false;
        }

        [RelayCommand]
        private async Task DeleteNode(NodeItemViewModel node)
        {
            if (node == null) return;
            var result = await _clusterService.DeleteClusterAsync(node.Id, CancellationToken.None);
            if (result.IsSuccess)
            {
                NodesList.Remove(node);
                ApplyFilterInternal();
            }
        }

        [RelayCommand]
        private void OpenAddDialog()
        {
            _editingNode = null;
            DialogTitle = "Добавление кластера";
            NewNodeName = "";
            NewNodeIp = "";
            IsAddDialogVisible = true;
        }
        [RelayCommand]
        private void EditNode(NodeItemViewModel node)
        {
            _editingNode = node;
            DialogTitle = "Редактирование кластера";
            NewNodeName = node.Name;
            NewNodeIp = node.Ip;
            IsAddDialogVisible = true;
        }
        [RelayCommand]
        private async Task ConfirmAddNode()
        {
            if (string.IsNullOrWhiteSpace(NewNodeName) || string.IsNullOrWhiteSpace(NewNodeIp)) return;
            if (_editingNode == null)
            {
                var request = new CreateClusterRequest(NewNodeName.Trim(), NewNodeIp.Trim());
                var result = await _clusterService.CreateClusterAsync(request, CancellationToken.None);
                if (result.IsSuccess && result.Data != null)
                {
                    NodesList.Add(new NodeItemViewModel(result.Data));
                    ApplyFilterInternal();
                }
            }
            else
            {
                var request = new UpdateClusterRequest(_editingNode.Id, NewNodeName.Trim(), NewNodeIp.Trim());
                var result = await _clusterService.UpdateClusterAsync(request, CancellationToken.None);
                if (result.IsSuccess && result.Data != null)
                {
                    _editingNode.Name = result.Data.Name;
                    _editingNode.Ip = result.Data.IpAddress;
                    _editingNode.Status = result.Data.Status;
                    _editingNode.Cpu = $"{result.Data.CpuUsagePercent:F0}% / {result.Data.CpuTotalPercent:F0}%";
                    _editingNode.Ram = $"{result.Data.RamUsageGb:F1} Гб / {result.Data.RamTotalGb:F1} Гб";
                }
            }
            IsAddDialogVisible = false;
        }
        [RelayCommand] private void CloseAddDialog() => IsAddDialogVisible = false;
    }

    public partial class NodeItemViewModel : ObservableObject
    {
        public Guid Id { get; set; }
        [ObservableProperty] private string _name = "";
        [ObservableProperty] private string _ip = "";
        [ObservableProperty] private ClusterStatus _status;
        [ObservableProperty] private string _cpu = "";
        [ObservableProperty] private string _ram = "";
        public string StatusText => Status == ClusterStatus.Online ? "В сети" : "Не в сети";

        public NodeItemViewModel() { }
        public NodeItemViewModel(ClusterDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Ip = dto.IpAddress;
            Status = dto.Status;
            Cpu = $"{dto.CpuUsagePercent:F0}% / {dto.CpuTotalPercent:F0}%";
            Ram = $"{dto.RamUsageGb:F1} Гб / {dto.RamTotalGb:F1} Гб";
        }
    }
}