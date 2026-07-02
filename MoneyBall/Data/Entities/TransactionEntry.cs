namespace MoneyBall.Data.Entities;

public enum TransactionType
{
    Deposit,
    Expense
}

public class TransactionEntry
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }

    // Only expenses are categorized; deposits leave this null.
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public required string Description { get; set; }
    public DateOnly OccurredOn { get; set; }

    public int CreatedByUserId { get; set; }
    public AppUser? CreatedByUser { get; set; }
    public DateTime CreatedAt { get; set; }
}
