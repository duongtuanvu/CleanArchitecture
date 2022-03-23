using Domain;
using Domain.Entities;
using Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public string UserId { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ExampleModel> ExampleModel { get; set; }
        public IDbConnection Connection => Database.GetDbConnection();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExampleModel>().HasQueryFilter(x => !x.IsDeleted);
        }

        public override int SaveChanges()
        {
            BeforeSaving();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            BeforeSaving();
            return base.SaveChangesAsync();
        }

        private void BeforeSaving()
        {
            var now = DateTime.Now;
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity);

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
                if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Unchanged;
                    ((BaseEntity)entityEntry.Entity).IsDeleted = true;
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
