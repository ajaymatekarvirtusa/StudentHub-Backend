using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    /// <summary>
    /// Column rules shared by every master table. Each master's configuration inherits this
    /// and only adds its own columns.
    /// </summary>
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            // Identity column: ValueGeneratedOnAdd is provider-neutral; SQL Server maps it to IDENTITY(1,1).
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            // No database default on IsActive: with a DB default of 1, EF would silently
            // insert "true" whenever the code sets IsActive = false.
            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.CreatedDate).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);

            // Soft-deleted rows are excluded from every query automatically.
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
