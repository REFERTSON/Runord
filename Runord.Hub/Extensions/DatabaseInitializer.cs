using Microsoft.EntityFrameworkCore;
using Runord.Hub.Data;
using Runord.Shared.Entities;
using Runord.Shared.Enums;

namespace Runord.Hub.Extensions
{
    public static class DatabaseInitializer
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Автоматический прогон миграций при старте
            await context.Database.MigrateAsync();

            // Проверка и создание дефолтного админа
            if (!context.Users.Any(u => u.Role == UserRole.Admin))
            {
                var admin = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@runord.com",
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    IsBlocked = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    LastModified = DateTimeOffset.UtcNow
                };

                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123");

                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
