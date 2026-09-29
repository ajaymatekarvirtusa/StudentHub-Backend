using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Configurations
{
    public class CountryConfiguration : BaseEntityConfiguration<Country>
    {
        public override void Configure(EntityTypeBuilder<Country> builder)
        {
            base.Configure(builder);

            builder.ToTable("Country", "dbo");

            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Code).HasMaxLength(3).IsUnicode(false).IsRequired();

            // Unique among non-deleted rows, so a deleted country's name/code can be reused.
            builder.HasIndex(c => c.Name).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(c => c.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
