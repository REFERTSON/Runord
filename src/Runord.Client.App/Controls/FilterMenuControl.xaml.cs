using Runord.Client.App.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Runord.Client.App.Controls
{
    /// <summary>
    /// Логика взаимодействия для FilterControl.xaml
    /// </summary>

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class FilterItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class FilterCategory
    {
        public string Name { get; set; }
        public ObservableCollection<FilterItem> Items { get; set; } = new();
    }

    public class SortField : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public SortDirection Direction { get; set; } // Asc / Desc

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); }
        }

        public ICommand ToggleSortDirectionCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class FilterControlViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FilterCategory> FilterCategories { get; set; } = new();
        public ObservableCollection<SortField> SortFields { get; set; } = new();

        public ICommand ApplyCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class TestFilterControlViewModel : FilterControlViewModel
    {
        public TestFilterControlViewModel()
        {
            // --- Фильтры ---
            FilterCategories.Add(new FilterCategory
            {
                Name = "Статус",
                Items = new ObservableCollection<FilterItem>
            {
                new FilterItem { Name = "Активный" },
                new FilterItem { Name = "Неактивный" },
                new FilterItem { Name = "Архив" }
            }
            });

            FilterCategories.Add(new FilterCategory
            {
                Name = "Тип",
                Items = new ObservableCollection<FilterItem>
            {
                new FilterItem { Name = "Базовый" },
                new FilterItem { Name = "Премиум" },
                new FilterItem { Name = "VIP" }
            }
            });

            FilterCategories.Add(new FilterCategory
            {
                Name = "Приоритет",
                Items = new ObservableCollection<FilterItem>
            {
                new FilterItem { Name = "Низкий" },
                new FilterItem { Name = "Средний" },
                new FilterItem { Name = "Высокий" }
            }
            });

            FilterCategories.Add(new FilterCategory
            {
                Name = "Приоритет 2",
                Items = new ObservableCollection<FilterItem>
            {
                new FilterItem { Name = "Низкий" },
                new FilterItem { Name = "Средний" },
                new FilterItem { Name = "Высокий" }
            }
            });

            FilterCategories.Add(new FilterCategory
            {
                Name = "Приоритет 2",
                Items = new ObservableCollection<FilterItem>
            {
                new FilterItem { Name = "Низкий" },
                new FilterItem { Name = "Средний" },
                new FilterItem { Name = "Высокий" }
            }
            });

            // --- Сортировка ---
            SortFields.Add(new SortField { Name = "Имя" });
            SortFields.Add(new SortField { Name = "Дата создания" });
            SortFields.Add(new SortField { Name = "Приоритет" });

            // Привязка команд для переключения направления сортировки
            foreach (var sort in SortFields)
            {
                sort.ToggleSortDirectionCommand = new RelayCommand((obj) =>
                {
                    sort.Direction = sort.Direction == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                });
            }

            // --- Команды Применить / Сбросить ---
            ApplyCommand = new RelayCommand((obj) =>
            {
                // Здесь логика применения фильтров
                var activeFilters = new List<string>();
                foreach (var cat in FilterCategories)
                {
                    foreach (var item in cat.Items)
                    {
                        if (item.IsSelected)
                            activeFilters.Add($"{cat.Name}: {item.Name}");
                    }
                }
                var sortings = SortFields.Select(f => $"{f.Name} ({f.Direction})").ToList();

                // Для проверки — вывод в консоль
                System.Diagnostics.Debug.WriteLine("Активные фильтры: " + string.Join(", ", activeFilters));
                System.Diagnostics.Debug.WriteLine("Сортировка: " + string.Join(", ", sortings));
            });

            ResetCommand = new RelayCommand((obj) =>
            {
                foreach (var cat in FilterCategories)
                    foreach (var item in cat.Items)
                        item.IsSelected = false;

                foreach (var sort in SortFields)
                    sort.Direction = SortDirection.Ascending;
            });
        }
    }


    public partial class FilterMenu : UserControl
    {
        public FilterMenu()
        {
            InitializeComponent();
            DataContext = new TestFilterControlViewModel();
        }
    }


}
