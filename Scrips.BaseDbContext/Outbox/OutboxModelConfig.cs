using Microsoft.EntityFrameworkCore;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Maps <see cref="OutboxMessage"/> into whatever context inherits the auditable base.
/// Call from the base contexts' OnModelCreating so every service gets one OutboxMessages table.
/// </summary>
public static class OutboxModelConfig
{
    public const string TableName = "OutboxMessages";

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.ToTable(TableName);
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.EventId).IsRequired();
            e.Property(x => x.TenantId).HasMaxLength(64);
            e.Property(x => x.Topic).HasMaxLength(200).IsRequired();
            e.Property(x => x.Payload).IsRequired(); // nvarchar(max)
            e.Property(x => x.Status).IsRequired();

            // Filtered index keeps the claim/reaper scans off the full (mostly Published) table.
            e.HasIndex(x => new { x.Status, x.Id })
                .HasDatabaseName("IX_OutboxMessages_Status_Id")
                .HasFilter("[Status] IN (0,1)");
        });
    }
}
