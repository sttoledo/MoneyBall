using Microsoft.EntityFrameworkCore;
using MoneyBall.Data.Entities;

namespace MoneyBall.Data;

public class MoneyBallDbContext(DbContextOptions<MoneyBallDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountAccess> AccountAccesses => Set<AccountAccess>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TransactionEntry> Transactions => Set<TransactionEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.Property(a => a.Type).HasConversion<string>();
            e.Property(a => a.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AccountAccess>(e =>
        {
            e.HasIndex(a => new { a.AccountId, a.UserId }).IsUnique();
            e.HasOne(a => a.Account).WithMany().HasForeignKey(a => a.AccountId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.Property(c => c.MonthlyBudget).HasPrecision(18, 2);
        });

        modelBuilder.Entity<TransactionEntry>(e =>
        {
            e.Property(t => t.Type).HasConversion<string>();
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasIndex(t => t.AccountId);
            e.HasOne(t => t.Account).WithMany().HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
