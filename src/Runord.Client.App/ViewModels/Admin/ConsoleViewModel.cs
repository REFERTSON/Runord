using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using Runord.Client.App.Base;

namespace Runord.Client.App.ViewModels
{
    public class ConsoleViewModel : ViewModelBase
    {
        private string _currentCommand = "";
        private readonly List<string> _commandHistory = new();
        private int _historyIndex = -1;

        public ConsoleViewModel()
        {
            ConsoleLines = new ObservableCollection<ConsoleLine>();
            ExecuteCommand = new RelayCommand(ExecuteCurrentCommand);
        }

        public ObservableCollection<ConsoleLine> ConsoleLines { get; }

        public string CurrentCommand
        {
            get => _currentCommand;
            set => SetProperty(ref _currentCommand, value);
        }

        public ICommand ExecuteCommand { get; }

        private void ExecuteCurrentCommand(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(CurrentCommand))
                return;

            // Добавляем команду в вывод (с зеленой стрелкой)
            ConsoleLines.Add(new ConsoleLine(CurrentCommand, true));

            // Обрабатываем команду
            ProcessCommand(CurrentCommand);

            // Добавляем в историю
            _commandHistory.Add(CurrentCommand);
            _historyIndex = _commandHistory.Count;

            // Очищаем поле ввода
            CurrentCommand = "";
        }

        private void ProcessCommand(string command)
        {
            switch (command.ToLower().Trim())
            {
                case "list tasks":
                    ConsoleLines.Add(new ConsoleLine("    Task 1: Project Alpha - Design 0Z-00 (Task: 2024-03-15)", false));
                    ConsoleLines.Add(new ConsoleLine("    Task 2: Project Alpha - Design 0Z-00 (Task: 2024-03-15)", false));
                    ConsoleLines.Add(new ConsoleLine(" ", false));
                    break;

                case "show logs":
                    ConsoleLines.Add(new ConsoleLine("   [2024-03-09 10:00:00] INFO Task: I started", false));
                    ConsoleLines.Add(new ConsoleLine("   [2024-03-09 10:00:00] INFO Task: I started", false));
                    ConsoleLines.Add(new ConsoleLine("   [2024-03-09 10:00:00] INFO Task: I started", false));
                    ConsoleLines.Add(new ConsoleLine(" ", false));
                    break;

                case "help":
                    ConsoleLines.Add(new ConsoleLine("Available commands:", false));
                    ConsoleLines.Add(new ConsoleLine("   list tasks - Show available tasks", false));
                    ConsoleLines.Add(new ConsoleLine("   show logs - Display task logs", false));
                    ConsoleLines.Add(new ConsoleLine("   help - Show this help message", false));
                    ConsoleLines.Add(new ConsoleLine("   clear - Clear console", false));
                    ConsoleLines.Add(new ConsoleLine(" ", false));
                    break;

                case "clear":
                    ConsoleLines.Clear();
                    break;

                default:
                    ConsoleLines.Add(new ConsoleLine($"Command not found: {command}", false));
                    ConsoleLines.Add(new ConsoleLine("Type 'help' for available commands", false));
                    break;
            }
        }
    }

    public class ConsoleLine
    {
        public string Text { get; set; }
        public bool IsCommand { get; set; }

        public ConsoleLine(string text, bool isCommand)
        {
            Text = text;
            IsCommand = isCommand;
        }
    }
}