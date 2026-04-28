using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task_Manager_API.Models;

namespace Task_Manager_API.Persistence.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("tasks");
            builder.HasKey(t=>t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
            builder.HasIndex(t => t.IsCompleted);
            builder.Property(t=>t.CreatedAt).HasColumnType("datetime2");
            builder.Property(t=>t.UpdatedAt).HasColumnType("datetime2");

            builder.Property(t => t.CreatedAt).IsRequired().ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            //builder.Property(t => t.UpdatedAt).ValueGeneratedOnUpdate();
        }
    }
}
