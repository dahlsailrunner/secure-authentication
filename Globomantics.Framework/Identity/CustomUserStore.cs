using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;
using Serilog;

namespace Globomantics.Framework.Identity
{
    public partial class CustomUserStore : IUserPasswordStore<CustomUser, int>, 
                                           IUserEmailStore<CustomUser, int>, 
                                           IUserSecurityStampStore<CustomUser, int>
    {
        private readonly IDbConnection _db;

        public CustomUserStore(IDbConnection db)
        {
            _db = db;
        }

        public async Task CreateAsync(CustomUser user)
        {
            user.CreateDate = DateTime.Now;
            user.PasswordModifiedDate = user.CreateDate;
            await _db.ExecuteAsync(
                @"
INSERT INTO GlobomanticsUser 
( LoginName, PasswordHash, PasswordModifiedDate, LastLoginDate, CreateDate, Status, SecurityStamp, EmailConfirmed, AccessFailedCount, LockoutEnd, LockoutEnabled )
VALUES
( @LoginName, @PasswordHash, @PasswordModifiedDate,@LastLoginDate, @CreateDate, 1, @SecurityStamp, @EmailConfirmed, @AccessFailedCount, LockoutEnd, LockoutEnabled )",
                user);
        }

        public async Task UpdateAsync(CustomUser user)
        {
            try
            {
                await _db.ExecuteAsync(
                    @"
UPDATE GlobomanticsUser 
SET PasswordHash = @PasswordHash 
   ,PasswordSalt = @PasswordSalt
   ,LoginName = @LoginName
   ,PasswordModifiedDate = @PasswordModifiedDate
   ,LastLoginDate = @LastLoginDate
   ,CreateDate = @CreateDate
   ,Status = @Status   
   ,SecurityStamp = @SecurityStamp
   ,EmailConfirmed = @EmailConfirmed
   ,TwoFactorEnabled = @TwoFactorEnabled
   ,AuthenticatorKey = @AuthenticatorKey
   ,AccessFailedCount = @AccessFailedCount
   ,LockoutEnd = @LockoutEnd
   ,LockoutEnabled = @LockoutEnabled
WHERE UserId = @UserId",
                    user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating identity.");
            }
        }

        public Task DeleteAsync(CustomUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomUser> FindByIdAsync(int userId)
        {
            return await _db.QuerySingleOrDefaultAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE UserId = @userId",
                new { userId });
        }

        public async Task<CustomUser> FindByNameAsync(string userName)
        {
            return await _db.QuerySingleOrDefaultAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE LoginName = @LoginName",
                new { LoginName = userName });
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            user.PasswordSalt = null;
            user.PasswordModifiedDate = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(CustomUser user)
        {
            return !string.IsNullOrEmpty(user.PasswordSalt)
                ? Task.FromResult($"{user.PasswordSalt}|{user.PasswordHash}") // | is not a base-64 character
                : Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(CustomUser user, string email)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(CustomUser user)
        {
            return Task.FromResult(user.LoginName);
        }

        public Task<bool> GetEmailConfirmedAsync(CustomUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(CustomUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<CustomUser> FindByEmailAsync(string email)
        {
            return FindByNameAsync(email);
        }
        public void Dispose() { }

        public Task SetSecurityStampAsync(CustomUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(CustomUser user)
        {
            return Task.FromResult(user.SecurityStamp ?? "");
        }
    }
}