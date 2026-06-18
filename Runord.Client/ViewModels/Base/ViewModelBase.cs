using CommunityToolkit.Mvvm.ComponentModel;

namespace Runord.Client.ViewModels.Base
{
    public abstract class ViewModelBase : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        protected async Task ExecuteAsync(Func<Task> action, string errorMessage = "Ошибка")
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{errorMessage}: {ex.Message}", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}