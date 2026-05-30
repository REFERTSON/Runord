using Microsoft.EntityFrameworkCore;
using Runord.Shared.Base;
using Runord.Shared.Entities;

namespace Runord.Hub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
        public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
        public DbSet<TaskFile> TaskFiles => Set<TaskFile>();
        public DbSet<ClusterEntity> Clusters => Set<ClusterEntity>();
        public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Конфигурация UserEntity (расширенная)
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Group).HasMaxLength(50);
                entity.Property(e => e.Role).HasConversion<int>();
                entity.Property(e => e.EmailConfirmationToken).HasMaxLength(256);
                entity.Property(e => e.PasswordResetToken).HasMaxLength(256);
                entity.Property(e => e.BlockReason).HasMaxLength(500);
                entity.Property(e => e.IsOnline).HasDefaultValue(false);
                entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
                entity.Property(e => e.IsBlocked).HasDefaultValue(false);
            });

            // 2. Конфигурация ProjectEntity
            modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);

                entity.HasOne(p => p.CreatedBy)
                      .WithMany(u => u.Projects)
                      .HasForeignKey(p => p.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Конфигурация TaskEntity
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => e.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(256);

                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Owner)
                      .WithMany(u => u.Tasks)
                      .HasForeignKey(t => t.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Конфигурация TaskFile
            modelBuilder.Entity<TaskFile>(entity =>
            {
                entity.ToTable("TaskFiles");
                entity.HasKey(e => e.Id);
                entity.Property(f => f.Name).IsRequired().HasMaxLength(512);
                entity.Property(f => f.Md5Hash).HasMaxLength(32);

                entity.HasOne(f => f.Task)
                      .WithMany(t => t.Files)
                      .HasForeignKey(f => f.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 5. Конфигурация ClusterEntity
            modelBuilder.Entity<ClusterEntity>(entity =>
            {
                entity.ToTable("Clusters");
                entity.HasKey(e => e.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(256);
                entity.Property(c => c.IpAddress).IsRequired().HasMaxLength(50);
            });

            // 6. Конфигурация NotificationEntity (UserId обязателен)
            modelBuilder.Entity<NotificationEntity>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(e => e.Id);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(256);
                entity.Property(n => n.Message).IsRequired();

                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 7. Конфигурация RefreshTokenEntity
            modelBuilder.Entity<RefreshTokenEntity>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.ReplacedByTokenId).HasMaxLength(256);

                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                var now = DateTimeOffset.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }

                entity.LastModified = now;
            }
        }
    }
}