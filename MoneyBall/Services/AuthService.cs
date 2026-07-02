using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyBall.Data;
using MoneyBall.Data.Entities;

namespace MoneyBall.Services;

public class AuthService(IDbContextFactory<MoneyBallDbContext> dbFactory)
{
    private readonly PasswordHasher<AppUser> _hasher = new();

    public async Task<AppUser?> ValidateCredentialsAsync(string username, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }
}
