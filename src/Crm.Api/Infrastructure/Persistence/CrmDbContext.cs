using Crm.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Infrastructure.Persistence;

public sealed class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Case> Cases => Set<Case>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("crm");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.Property(x => x.Industry).HasMaxLength(120);
            entity.Property(x => x.Website).HasMaxLength(250);
            entity.Property(x => x.PhoneNumber).HasMaxLength(40);
            entity.Property(x => x.BillingCity).HasMaxLength(120);
            entity.Property(x => x.BillingCountry).HasMaxLength(120);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(120);
            entity.Property(x => x.LastName).HasMaxLength(120);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.PhoneNumber).HasMaxLength(40);
            entity.Property(x => x.JobTitle).HasMaxLength(150);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.HasIndex(x => x.Email).IsUnique().HasFilter("\"Email\" IS NOT NULL");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(150);
            entity.Property(x => x.CompanyName).HasMaxLength(200);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.PhoneNumber).HasMaxLength(40);
            entity.Property(x => x.Source).HasMaxLength(100);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.Property(x => x.Notes).HasMaxLength(4000);
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.Property(x => x.EstimatedValue).HasPrecision(18, 2);
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Opportunities)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.PrimaryContact)
                .WithMany(x => x.Opportunities)
                .HasForeignKey(x => x.PrimaryContactId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(x => x.Summary).HasMaxLength(200);
            entity.Property(x => x.Details).HasMaxLength(4000);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.HasIndex(x => new { x.ParentType, x.ParentId, x.OccurredAtUtc });
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.HasIndex(x => new { x.ParentType, x.ParentId, x.DueDate });
        });

        modelBuilder.Entity<Case>(entity =>
        {
            entity.Property(x => x.Subject).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.OwnerName).HasMaxLength(120);
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Cases)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Contact)
                .WithMany(x => x.Cases)
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Contact>()
            .HasOne(x => x.Account)
            .WithMany(x => x.Contacts)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = timestamp;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = timestamp;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
