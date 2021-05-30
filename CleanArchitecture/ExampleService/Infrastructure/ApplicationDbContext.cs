using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public Guid UserId { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ExampleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
        }

        public override int SaveChanges()
        {
            BeforeSaving();
            Delete();
            return base.SaveChanges();
        }

        private void BeforeSaving()
        {
            var now = DateTime.Now;
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = now;
                    ((BaseEntity)entityEntry.Entity).CreatedBy = UserId;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    ((BaseEntity)entityEntry.Entity).UpdatedDate = now;
                    ((BaseEntity)entityEntry.Entity).UpdatedBy = UserId;
                }
            };
        }

        private void Delete()
        {
            ChangeTracker.DetectChanges();
            var markedAsDeleted = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
            foreach (var item in markedAsDeleted)
            {
                if (item.Entity is BaseEntity entity)
                {
                    item.State = EntityState.Unchanged;
                    item.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                }
            }
        }
    }
}
