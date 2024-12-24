using HgznMes.Domain.Entities;
using HgznMes.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using CaseExtensions;
using HgznMes.Domain.Entities.Account;
using HgznMes.Domain.Entities.Authority;
using HgznMes.Domain.Entities.Location;
using HgznMes.Domain.Entities.Base.Audited;
using Microsoft.AspNetCore.Http;
using HgznMes.Domain.Shared;

namespace HgznMes.Infrastructure.DbContexts
{
    public class ApiDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiDbContext(
            DbContextOptions<ApiDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region dbsets

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Scope> Scopes { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        
        public DbSet<BuildingAggregateRoot> Building { get; set; } = null!;
        public DbSet<RoomEntity> Rooms { get; set; } = null!;
        public DbSet<FloorEntity> Floors { get; set; } = null!;

        #endregion dbsets

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.State == EntityState.Deleted)
                {
                    if (entityEntry.Entity is ISoftDelete delete)
                    {
                        entityEntry.State = EntityState.Unchanged;
                        delete.SoftDeleted = true;
                        delete.DeleteTime = DateTime.UtcNow;
                    }
                    if (entityEntry.Entity is ICreationAudited creationAudited)
                    {
                        creationAudited.CreationTime =DateTime.Now;
                        creationAudited.CreatorId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(c => CustomClaimsType.UserId == c.Type).Value);
                    }
                    if (entityEntry.Entity is ILastModificationAudited lastModificationAudited)
                    {
                        lastModificationAudited.LastModificationTime = DateTime.Now;
                        lastModificationAudited.LastModifierId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(c => CustomClaimsType.UserId == c.Type).Value);
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region table prefix

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.ClrType.Name.ToSnakeCase());
            }

            #endregion table prefix

            #region soft delete filter

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
            {
                //Expression<Func<ISoftDelete, bool>> filter = x => !x.SoftDeleted;
                //entityType.SetQueryFilter(filter);
                entityType.AddSoftDeleteQueryFilter();
            }

            #endregion soft delete filter

            #region entities initialize

            #region role

            modelBuilder.Entity<Role>()
                .HasData(Role.Seeds);

            modelBuilder.Entity<Role>()
                .HasIndex(u => u.SoftDeleted);

            modelBuilder.Entity<Role>()
                .HasIndex(r => new { r.Name, r.SoftDeleted })
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Scopes)
                .WithOne(s => s.Role)
                .IsRequired(false)
                .HasForeignKey(s => s.RoleId);

            modelBuilder.Entity<Role>()
                .HasMany(u => u.Users)
                .WithOne(b => b.Role)
                .HasForeignKey(b => b.RoleId);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Menus)
                .WithMany()
                .UsingEntity<RoleMenu>();

            #endregion role

            #region scope

            modelBuilder.Entity<Scope>()
                .HasData(Scope.Seeds);

            #endregion scope

            #region user

            modelBuilder.Entity<User>()
                .HasData(User.Seeds);

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Username, u.SoftDeleted })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.SoftDeleted);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Settings);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Detail);

            #endregion user

            #region menu

            modelBuilder.Entity<Menu>()
                .HasIndex(m => m.Order);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany()
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Menu>()
                .HasData(Menu.Seeds);

            modelBuilder.Entity<Menu>()
                .HasIndex(m => m.Code)
                .IsUnique();

            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Menu)
                .WithMany()
                .HasForeignKey(rm => rm.MenuId);

            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Role)
                .WithMany()
                .HasForeignKey(rm => rm.RoleId);
            
            #endregion

            #region Location

            modelBuilder.Entity<BuildingAggregateRoot>()
                .HasMany(f => f.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FloorEntity>()
                .HasOne(t => t.Building)
                .WithMany()
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FloorEntity>()
                .HasMany(t => t.Rooms)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<RoomEntity>()
                .HasOne(t => t.Floor)
                .WithMany()
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #endregion entities initialize
        }
    }
}