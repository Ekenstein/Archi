using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Archi.Models.EF
{
    public class ArchiContext : DbContext
    {
        public DbSet<Archive> Archives { get; set; }
        public DbSet<ArchiveFile> ArchiveFiles { get; set; }
        public DbSet<ArchiveTag> ArchiveTags { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public ArchiContext(DbContextOptions<ArchiContext> contextOptions) : base(contextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Archive>(b =>
            {
                b.HasKey(p => p.Id);
                b.HasMany(p => p.Files).WithOne(p => p.Archive).HasForeignKey(p => p.ArchiveId);
                b.HasMany(p => p.Tags).WithOne(p => p.Archive).HasForeignKey(p => p.ArchiveId);
                b.Property<bool>("IsDeleted");
                b.HasQueryFilter(p => !Microsoft.EntityFrameworkCore.EF.Property<bool>(p, "IsDeleted"));
                b.Property(p => p.Created).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ArchiveFile>(b =>
            {
                b.HasKey(p => new {p.ArchiveId, p.FileId});
                b.HasOne(p => p.Archive).WithMany(p => p.Files);
                b.HasOne(p => p.File).WithMany().HasForeignKey(p => p.FileId);
            });

            modelBuilder.Entity<ArchiveTag>(b =>
            {
                b.HasKey(p => new {p.TagId, p.ArchiveId});
                b.HasOne(p => p.Archive).WithMany(p => p.Tags).HasForeignKey(p => p.ArchiveId);
                b.HasOne(p => p.Tag).WithMany().HasForeignKey(p => p.TagId);
            });

            modelBuilder.Entity<Tag>(b =>
            {
                b.Property(p => p.Name).IsRequired();
                b.HasKey(p => p.Id);
            });

            modelBuilder.Entity<File>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.ContentType).IsRequired();
                b.Property(p => p.FileName).IsRequired();
            });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        OnDeleted(entry);
                        break;
                    case EntityState.Added:
                        OnAdded(entry);
                        break;
                }
            }
        }

        /// <summary>
        /// Called before an entry is about to be deleted.
        /// </summary>
        /// <param name="entry">The entry that is about to be deleted.</param>
        protected virtual void OnDeleted(EntityEntry entry)
        {
            if (!HasSoftDelete(entry))
            {
                return;
            }

            entry.State = EntityState.Modified;
            entry.CurrentValues["IsDeleted"] = true;
        }

        /// <summary>
        /// Called before an entry is about to be added.
        /// </summary>
        /// <param name="entry">The entry that is about to be added.</param>
        protected virtual void OnAdded(EntityEntry entry)
        {
            if (!HasSoftDelete(entry))
            {
                return;
            }

            entry.CurrentValues["IsDeleted"] = false;
        }

        private bool HasSoftDelete(EntityEntry entry) => entry
            .CurrentValues
            .Properties
            .Any(p => p.Name == "IsDeleted");
    }
}
