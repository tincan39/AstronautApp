using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("Logs")]
    public class LogEntry
    {
        public const string StatusSuccess = "Success";
        public const string StatusError = "Error";

        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RequestName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long ElapsedMs { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
    }

    public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.RequestName).IsRequired();
            builder.Property(x => x.Status).IsRequired();
        }
    }
}
