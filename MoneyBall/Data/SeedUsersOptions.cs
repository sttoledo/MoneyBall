namespace MoneyBall.Data;

public class SeedUsersOptions
{
    public const string SectionName = "SeedUsers";

    public required SeedUser Admin { get; set; }
    public required SeedUser Restricted { get; set; }

    public class SeedUser
    {
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
        public required string Password { get; set; }
    }
}
