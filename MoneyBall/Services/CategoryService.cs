using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MoneyBall.Data;
using MoneyBall.Data.Entities;

namespace MoneyBall.Services;

public record CategoryBudgetSummary(int CategoryId, string Name, decimal? Budget, decimal Spent)
{
    public decimal? Remaining => Budget - Spent;
}

public class CategoryService(IDbContextFactory<MoneyBallDbContext> dbFactory, AccountService accountService)
{
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task CreateCategoryAsync(string name, decimal? monthlyBudget)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Categories.Add(new Category { Name = name, MonthlyBudget = monthlyBudget, IsActive = true });
        await db.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(int id, string name, decimal? monthlyBudget)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var category = await db.Categories.FirstAsync(c => c.Id == id);
        category.Name = name;
        category.MonthlyBudget = monthlyBudget;
        await db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var category = await db.Categories.FirstAsync(c => c.Id == id);
        category.IsActive = isActive;
        await db.SaveChangesAsync();
    }

    public async Task<List<CategoryBudgetSummary>> GetBudgetSummaryAsync(ClaimsPrincipal user, int year, int month)
    {
        var visibleAccountIds = (await accountService.GetVisibleAccountsAsync(user)).Select(a => a.Id).ToList();
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1);

        await using var db = await dbFactory.CreateDbContextAsync();

        var expenses = await db.Transactions
            .Where(t => t.Type == TransactionType.Expense
                        && t.CategoryId != null
                        && visibleAccountIds.Contains(t.AccountId)
                        && t.OccurredOn >= monthStart
                        && t.OccurredOn < monthEnd)
            .Select(t => new { t.CategoryId, t.Amount })
            .ToListAsync();

        // SQLite's EF Core provider can't translate Sum(decimal) server-side, so aggregate client-side.
        var spentByCategory = expenses
            .GroupBy(t => t.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        // Include inactive categories too if they had spend in this month, so browsing a past
        // month doesn't silently drop spending under a category deactivated since then.
        var categories = (await db.Categories.OrderBy(c => c.Name).ToListAsync())
            .Where(c => c.IsActive || spentByCategory.ContainsKey(c.Id))
            .ToList();

        return categories
            .Select(c => new CategoryBudgetSummary(c.Id, c.Name, c.MonthlyBudget, spentByCategory.GetValueOrDefault(c.Id)))
            .ToList();
    }
}
