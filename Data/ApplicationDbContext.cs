using Microsoft.EntityFrameworkCore;
using turbo_funicular.Models;

namespace turbo_funicular.Data 
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<User> Users { set; get; }
        public DbSet<Group> Groups { set; get; }
        public DbSet<Message> Messages { set; get; }
        public DbSet<Event> Events { set; get; }
        public DbSet<UserGroup> UserGroups { set; get; }
        public DbSet<UserEvent> UserEvents { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreateDate).IsRequired().HasColumnType("datetime");

                entity.HasMany(e => e.OwnedGroups)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.OwnedEvents)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.UserGroups)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.UserEvents)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Messages)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.CreateTime).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdateTime).IsRequired().HasColumnType("datetime");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.OwnedGroups)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.UserGroups)
                    .WithOne(e => e.Group)
                    .HasForeignKey(e => e.GroupId);

                entity.HasMany(e => e.Messages)
                    .WithOne(e => e.Group)
                    .HasForeignKey(e => e.GroupId);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired().HasColumnType("datetime");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.Messages)
                    .HasForeignKey(e => e.GroupId);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.CreateDate).IsRequired().HasColumnType("datetime");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.OwnedEvents)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.UserEvents)
                    .WithOne(e => e.Event)
                    .HasForeignKey(e => e.EventId);
            });

            modelBuilder.Entity<UserEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserEvents)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.UserEvents)
                    .HasForeignKey(e => e.EventId);
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserGroups)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.UserGroups)
                    .HasForeignKey(e => e.GroupId);
            });
        }
    }
}