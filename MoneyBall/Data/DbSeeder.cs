using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoneyBall.Data.Entities;

namespace MoneyBall.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MoneyBallDbContext db, IOptions<SeedUsersOptions> seedOptions)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var hasher = new PasswordHasher<AppUser>();
        var options = seedOptions.Value;

        var admin = new AppUser
        {
            Username = options.Admin.Username,
            DisplayName = options.Admin.DisplayName,
            Role = UserRole.Admin,
            PasswordHash = string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, options.Admin.Password);

        var restricted = new AppUser
        {
            Username = options.Restricted.Username,
            DisplayName = options.Restricted.DisplayName,
            Role = UserRole.Restricted,
            PasswordHash = string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        restricted.PasswordHash = hasher.HashPassword(restricted, options.Restricted.Password);

        db.Users.AddRange(admin, restricted);

        var mainAccount = new Account { Name = "Main", Type = AccountType.Other, Balance = 0m, CreatedAt = DateTime.UtcNow };
        var savingsAccount = new Account { Name = "Savings", Type = AccountType.Savings, Balance = 0m, CreatedAt = DateTime.UtcNow };
        var checkingAccount = new Account { Name = "Checking", Type = AccountType.Checking, Balance = 0m, CreatedAt = DateTime.UtcNow };

        db.Accounts.AddRange(mainAccount, savingsAccount, checkingAccount);

        db.Categories.AddRange(
            new Category { Name = "Groceries" },
            new Category { Name = "Utilities" },
            new Category { Name = "Dining" },
            new Category { Name = "Transportation" },
            new Category { Name = "Other" });

        await db.SaveChangesAsync();

        db.AccountAccesses.AddRange(
            new AccountAccess { AccountId = savingsAccount.Id, UserId = restricted.Id },
            new AccountAccess { AccountId = checkingAccount.Id, UserId = restricted.Id });

        await db.SaveChangesAsync();
    }
}
