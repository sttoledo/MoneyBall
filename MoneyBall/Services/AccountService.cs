using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MoneyBall.Data;
using MoneyBall.Data.Entities;

namespace MoneyBall.Services;

public class AccountService(IDbContextFactory<MoneyBallDbContext> dbFactory)
{
    public async Task<List<Account>> GetVisibleAccountsAsync(ClaimsPrincipal user)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var isAdmin = user.IsInRole(nameof(UserRole.Admin));
        if (isAdmin)
        {
            return await db.Accounts.Where(a => a.IsActive).OrderBy(a => a.Name).ToListAsync();
        }

        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await db.AccountAccesses
            .Where(aa => aa.UserId == userId)
            .Select(aa => aa.Account!)
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Account?> GetAccountAsync(int id, ClaimsPrincipal user)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var isAdmin = user.IsInRole(nameof(UserRole.Admin));
        if (isAdmin)
        {
            return await db.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await db.AccountAccesses
            .Where(aa => aa.UserId == userId && aa.AccountId == id)
            .Select(aa => aa.Account!)
            .FirstOrDefaultAsync(a => a.IsActive);
    }
}
