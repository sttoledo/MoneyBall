namespace MoneyBall.Data.Entities;

public enum AccountType
{
    Checking,
    Savings,
    Other,
    CreditCard
}

public class Account
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
