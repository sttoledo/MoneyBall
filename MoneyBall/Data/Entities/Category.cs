namespace MoneyBall.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Null means no budget has been set for this category.
    public decimal? MonthlyBudget { get; set; }
    public bool IsActive { get; set; } = true;
}
