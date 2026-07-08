using Microsoft.EntityFrameworkCore;
using MoneyBall.Data;
using MoneyBall.Data.Entities;

namespace MoneyBall.Services;

public class TransactionService(IDbContextFactory<MoneyBallDbContext> dbFactory)
{
    public async Task AddDepositAsync(int accountId, decimal amount, string description, DateOnly occurredOn, int userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var account = await db.Accounts.FirstAsync(a => a.Id == accountId);

        account.Balance += amount;
        db.Transactions.Add(new TransactionEntry
        {
            AccountId = accountId,
            CategoryId = null,
            Type = TransactionType.Deposit,
            Amount = amount,
            Description = description,
            OccurredOn = occurredOn,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task AddExpenseAsync(int accountId, int categoryId, decimal amount, string description, DateOnly occurredOn, int userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var account = await db.Accounts.FirstAsync(a => a.Id == accountId);

        account.Balance -= amount;
        db.Transactions.Add(new TransactionEntry
        {
            AccountId = accountId,
            CategoryId = categoryId,
            Type = TransactionType.Expense,
            Amount = amount,
            Description = description,
            OccurredOn = occurredOn,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(int transactionId, decimal amount, string description, DateOnly occurredOn, int? categoryId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var txn = await db.Transactions.FirstAsync(t => t.Id == transactionId);
        var account = await db.Accounts.FirstAsync(a => a.Id == txn.AccountId);

        var oldDelta = txn.Type == TransactionType.Deposit ? txn.Amount : -txn.Amount;
        var newDelta = txn.Type == TransactionType.Deposit ? amount : -amount;
        account.Balance += newDelta - oldDelta;

        txn.Amount = amount;
        txn.Description = description;
        txn.OccurredOn = occurredOn;
        if (txn.Type == TransactionType.Expense)
        {
            txn.CategoryId = categoryId;
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var txn = await db.Transactions.FirstAsync(t => t.Id == transactionId);
        var account = await db.Accounts.FirstAsync(a => a.Id == txn.AccountId);

        var delta = txn.Type == TransactionType.Deposit ? txn.Amount : -txn.Amount;
        account.Balance -= delta;

        db.Transactions.Remove(txn);
        await db.SaveChangesAsync();
    }

    public async Task<List<TransactionEntry>> GetTransactionsAsync(int accountId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Transactions
            .Include(t => t.Category)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.OccurredOn)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveCategoriesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Categories.Where(c => c.IsActive && !c.IsBudgetExempt).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task PayCreditCardAsync(int fromAccountId, int creditCardAccountId, decimal amount, string description, DateOnly occurredOn, int userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var paymentCategory = await db.Categories.FirstAsync(c => c.IsBudgetExempt);
        var fromAccount = await db.Accounts.FirstAsync(a => a.Id == fromAccountId);
        var creditCardAccount = await db.Accounts.FirstAsync(a => a.Id == creditCardAccountId);
        var now = DateTime.UtcNow;

        fromAccount.Balance -= amount;
        db.Transactions.Add(new TransactionEntry
        {
            AccountId = fromAccountId,
            CategoryId = paymentCategory.Id,
            Type = TransactionType.Expense,
            Amount = amount,
            Description = description,
            OccurredOn = occurredOn,
            CreatedByUserId = userId,
            CreatedAt = now
        });

        creditCardAccount.Balance += amount;
        db.Transactions.Add(new TransactionEntry
        {
            AccountId = creditCardAccountId,
            CategoryId = null,
            Type = TransactionType.Deposit,
            Amount = amount,
            Description = description,
            OccurredOn = occurredOn,
            CreatedByUserId = userId,
            CreatedAt = now
        });

        await db.SaveChangesAsync();
    }
}
