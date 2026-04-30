using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Manager_API.Models;

namespace Task_Manager_API.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
            builder.Property(u => u.PasswordHash).IsRequired().HasColumnType("nvarchar(500)");
            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
