using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.BackgroundServices
{
    public class TokenCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TokenCleanupWorker> _logger;

        // Интервал между последующими проверками (например, каждые 6 часов)
        private readonly TimeSpan _period = TimeSpan.FromHours(6);

        public TokenCleanupWorker(IServiceScopeFactory scopeFactory, ILogger<TokenCleanupWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Смотритель токенов запущен.");

            // 1. ЗАПУСК В РЕАЛЬНОМ ВРЕМЕНИ: Выполняем очистку сразу при старте сервера, не дожидаясь таймера
            await DeleteTokensSafeAsync(stoppingToken);

            // 2. Инициализируем таймер для последующих периодических проверок
            using var timer = new PeriodicTimer(_period);

            // Цикл будет спать и просыпаться каждые 6 часов
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DeleteTokensSafeAsync(stoppingToken);
            }
        }

        /// <summary>
        /// Метод для безопасного выполнения очистки токенов в отдельной области видимости (Scope)
        /// </summary>
        private async Task DeleteTokensSafeAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Смотритель инициирует очистку базы от устаревших и отозванных токенов...");

                // Так как BackgroundService — это Singleton, создаем Scope для работы со Scoped-репозиториями
                using var scope = _scopeFactory.CreateScope();
                var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                // Вызов метода удаления из репозитория
                await refreshTokenRepository.DeleteExpiredAndRevokedTokensAsync(stoppingToken);

                _logger.LogInformation("Смотритель успешно завершил очистку.");
            }
            catch (Exception ex)
            {
                // Логируем ошибку, чтобы падение базы данных или сети не обрушило сам сервер
                _logger.LogError(ex, "Во время работы смотрителя токенов произошла ошибка.");
            }
        }
    }
}