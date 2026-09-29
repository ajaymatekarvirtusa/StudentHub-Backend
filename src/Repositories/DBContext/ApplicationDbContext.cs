using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.DBContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUser)
        : IdentityDbContext<ApplicationUser>(options)
    {
        // ---- Masters (add one line per new master, e.g. public DbSet<State> States => Set<State>();) ----
        public DbSet<Country> Countries => Set<Country>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Picks up every IEntityTypeConfiguration in this project (CountryConfiguration, ...).
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditFields();
            return base.SaveChanges();
        }

        /// <summary>Fills Created*/Modified* for every master automatically.</summary>
        private void ApplyAuditFields()
        {
            var now = DateTime.UtcNow;
            var user = currentUser.UserName;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        entry.Entity.CreatedBy = user;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = now;
                        entry.Entity.ModifiedBy = user;
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;
                        break;
                }
            }
        }
    }
}
