namespace MoneyBall.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Null means no budget has been set for this category.
    public decimal? MonthlyBudget { get; set; }
    public bool IsActive { get; set; } = true;

    // Excluded from budget totals and the normal expense-entry dropdown. Used for
    // reserved system categories like "Credit Card Payment" that aren't real spending.
    public bool IsBudgetExempt { get; set; }
}
