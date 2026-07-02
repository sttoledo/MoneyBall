namespace MoneyBall.Data.Entities;

// Only Restricted users get rows here; Admin users see every account regardless.
public class AccountAccess
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}
