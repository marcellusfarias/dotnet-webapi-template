using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;

namespace MyBuyingList.Infrastructure.Persistence.Seeders;

public class AdminUserSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordEncryptionService _passwordService;
    private readonly AdminSettings _adminSettings;

    public AdminUserSeeder(
        ApplicationDbContext db,
        IPasswordEncryptionService passwordService,
        IOptions<AdminSettings> adminSettings)
    {
        _db = db;
        _passwordService = passwordService;
        _adminSettings = adminSettings.Value;
    }

    public async Task SeedAsync()
    {
        var existingUser = await _db.Set<User>().FindAsync(Users.AdminUser.Id);

        if (existingUser is null)
        {
            var hashedPassword = _passwordService.HashPassword(_adminSettings.Password);

            // UseIdentityAlwaysColumn prevents EF from supplying an Id, so we use raw SQL
            // with OVERRIDING SYSTEM VALUE to insert the admin user with its fixed Id = 1.
            await _db.Database.ExecuteSqlRawAsync(
                "INSERT INTO users (id, user_name, email, password, active) OVERRIDING SYSTEM VALUE VALUES ({0}, {1}, {2}, {3}, {4})",
                Users.AdminUser.Id, Users.AdminUser.UserName, _adminSettings.Email, hashedPassword, true);

            // Advance the identity sequence so subsequent auto-generated ids don't collide with the manually-set id.
            await _db.Database.ExecuteSqlRawAsync(
                "SELECT setval(pg_get_serial_sequence('users', 'id'), (SELECT MAX(id) FROM users))");

            await _db.Database.ExecuteSqlRawAsync(
                "INSERT INTO user_roles (user_id, role_id) VALUES ({0}, {1})",
                Users.AdminUser.Id, 1);
        }
        else
        {
            var changed = false;

            if (!_passwordService.VerifyPassword(_adminSettings.Password, existingUser.Password))
            {
                existingUser.Password = _passwordService.HashPassword(_adminSettings.Password);
                changed = true;
            }

            if (existingUser.Email != _adminSettings.Email)
            {
                existingUser.Email = _adminSettings.Email;
                changed = true;
            }

            if (changed)
            {
                await _db.SaveChangesAsync();
            }
        }
    }
}
