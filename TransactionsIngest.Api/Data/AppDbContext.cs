
using Microsoft.EntityFrameworkCore;
using TransactionsIngest.Api.Models;

namespace TransactionsIngest.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TransactionRecord> Transactions { get; set; }

    public DbSet<TransactionAudit> TransactionAudits { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var transaction = modelBuilder.Entity<TransactionRecord>();

        transaction
            .HasIndex(t => new { t.SourceSystem, t.TransactionId })
            .IsUnique();

        transaction.Property(t => t.SourceSystem)
            .HasMaxLength(19)
            .IsRequired();

        transaction.Property(t => t.CardNumber)
            .HasMaxLength(19)
            .IsRequired();

        transaction.Property(t => t.LocationCode)
            .HasMaxLength(19)
            .IsRequired();

        transaction.Property(t => t.ProductName)
            .HasMaxLength(19)
            .IsRequired();

        transaction.Property(t => t.Amount)
            .HasPrecision(18, 2);


        modelBuilder.Entity<TransactionAudit>()
            .HasOne(a => a.TransactionRecord)
            .WithMany()
            .HasForeignKey(a => a.TransactionRecordId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
