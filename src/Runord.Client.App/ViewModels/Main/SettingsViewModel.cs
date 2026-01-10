using Runord.Client.App.Base;
using Runord.Client.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<SettingItem> GeneralSettings { get; set; }
        public ObservableCollection<SettingItem> NotificationSettings { get; set; }
        public ObservableCollection<SettingItem> InterfaceSettings { get; set; }
        public ObservableCollection<SettingItem> NetworkSettings { get; set; }
        public ObservableCollection<SettingItem> NodeSettings { get; set; }
        public ObservableCollection<SettingItem> SecuritySettings { get; set; }

        public SettingsViewModel()
        {
            GeneralSettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Тема", Description = "Светлая / Тёмная / Авто", Value = "Dark", Options = new List<string>{ "Light", "Dark", "Auto" }, Type = SettingType.Option },
            new SettingItem { Name = "Язык интерфейса", Description = "Выбор языка", Value = "Русский", Options = new List<string>{ "Русский", "English" }, Type = SettingType.Option },
            new SettingItem { Name = "Автозапуск", Description = "Запускать при старте системы", Value = true, Type = SettingType.Boolean }
        };

            NotificationSettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Показывать уведомления", Description = "", Value = true, Type = SettingType.Boolean },
            new SettingItem { Name = "Звук уведомлений", Description = "", Value = true, Type = SettingType.Boolean },
            new SettingItem { Name = "Всплывающие подсказки", Description = "", Value = true, Type = SettingType.Boolean }
        };

            InterfaceSettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Размер шрифта", Description = "", Value = "Средний", Options = new List<string>{ "Малый", "Средний", "Большой" }, Type = SettingType.Option },
            new SettingItem { Name = "Анимации интерфейса", Description = "", Value = true, Type = SettingType.Boolean }
        };

            NetworkSettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Автосинхронизация", Description = "", Value = true, Type = SettingType.Boolean },
            new SettingItem { Name = "Интервал синхронизации (мин)", Description = "", Value = 5, Type = SettingType.Number }
        };

            NodeSettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Тип отображения нод", Description = "", Value = "Плитки", Options = new List<string>{ "Плитки", "Список" }, Type = SettingType.Option },
            new SettingItem { Name = "Подсветка при изменении", Description = "", Value = true, Type = SettingType.Boolean }
        };

            SecuritySettings = new ObservableCollection<SettingItem>
        {
            new SettingItem { Name = "Пароль для доступа", Description = "", Value = "", Type = SettingType.String },
            new SettingItem { Name = "Блокировка при сворачивании", Description = "", Value = true, Type = SettingType.Boolean }
        };
        }
    }
}
