using CaseExtensions;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hgzn.Mes.Infrastructure.DbContexts.Ef
{
    public class ApiDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApiDbContext(
            DbContextOptions<ApiDbContext> options,
            IHttpContextAccessor? httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region dbsets

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;

        public DbSet<Building> Building { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Floor> Floors { get; set; } = null!;

        public DbSet<EquipLedger> EquipLedgers { get; set; } = null!;
        public DbSet<EquipType> EquipTypes { get; set; } = null!;

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
                        delete.DeleteTime = DateTime.Now.ToLocalTime();
                    }
                }
                if (entityEntry.Entity is ICreationAudited creationAudited)
                {
                    var plain = _httpContextAccessor?.HttpContext?.User.Claims
                        .First(c => ClaimType.UserId == c.Type).Value;
                    var userId = plain is null ? new Guid?() : Guid.Parse(plain);
                    creationAudited.CreationTime = DateTime.Now.ToLocalTime();
                    creationAudited.CreatorId = userId;
                }
                if (entityEntry.Entity is ILastModificationAudited lastModificationAudited)
                {
                    var plain = _httpContextAccessor?.HttpContext?.User.Claims
                        .First(c => ClaimType.UserId == c.Type).Value;
                    var userId = plain is null ? new Guid?() : Guid.Parse(plain);
                    lastModificationAudited.LastModificationTime = DateTime.Now.ToLocalTime();
                    lastModificationAudited.LastModifierId = userId;
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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Menus)
                .WithMany()
                .UsingEntity<RoleMenu>();

            #endregion role

            #region menu

            modelBuilder.Entity<RoleMenu>()
                .HasData(RoleMenu.Seeds);

            #endregion menu

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

            // modelBuilder.Entity<User>()
            //     .OwnsOne(u => u.Detail);

            modelBuilder.Entity<UserRole>()
                .HasData(UserRole.Seeds);

            #endregion user

            #region menu

            modelBuilder.Entity<Menu>()
                .HasIndex(m => m.OrderNum);

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

            #endregion menu

            #region Location

            modelBuilder.Entity<Building>()
                .HasMany(f => f.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Floor>()
                .HasOne(t => t.Building)
                .WithMany()
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Floor>()
                .HasMany(t => t.Rooms)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(t => t.Floor)
                .WithMany()
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion Location

            #endregion entities initialize
        }
    }
}