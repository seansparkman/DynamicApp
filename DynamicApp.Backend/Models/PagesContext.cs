using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicApp.Backend.Models
{
    public class PagesContext : DbContext
    {
        public PagesContext(DbContextOptions<PagesContext> options)
            : base(options)
        {
        }

        public DbSet<MobilePage> Pages { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Layout> Layouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MobilePage>()
                .Property(p => p.Guid)
                .HasDefaultValueSql("newsequentialid()");

            modelBuilder.Entity<MobilePage>()
                .Property(p => p.Updated)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<MobilePage>()
                .Property(p => p.Created)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Section>()
                .Property(p => p.Guid)
                .HasDefaultValueSql("newsequentialid()");

            modelBuilder.Entity<Section>()
                .Property(p => p.Updated)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Section>()
                .Property(p => p.Created)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Layout>()
                .Property(p => p.Guid)
                .HasDefaultValueSql("newsequentialid()");

            modelBuilder.Entity<Layout>()
                .Property(p => p.Updated)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Layout>()
                .Property(p => p.Created)
                .HasDefaultValueSql("getdate()");
        }

        //public override int SaveChanges()
        //{
        //    AddTimestamps();
        //    return base.SaveChanges();
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    AddTimestamps();
        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        //private void AddTimestamps()
        //{
        //    var entities = ChangeTracker.Entries<BaseEntity>()
        //        .Where(x => x.State == EntityState.Added ||
        //            x.State == EntityState.Modified);

        //    foreach (var entity in entities)
        //    {
        //        if (entity.State == EntityState.Added)
        //        {
        //            ((BaseEntity)entity.Entity).Created = DateTime.UtcNow;
        //            ((BaseEntity)entity.Entity).Guid = Guid.NewGuid();
        //        }

        //        ((BaseEntity)entity.Entity).Updated = DateTime.UtcNow;
        //    }
        //}
    }
}
