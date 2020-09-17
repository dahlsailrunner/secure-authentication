using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;
using Serilog;

namespace Globomantics.Framework.Models
{
    public partial class CustomUserStore : IUserPasswordStore<CustomUser, int>,
                                   IUserEmailStore<CustomUser, int>
    {
        private readonly IDbConnection _db;

        public CustomUserStore(IDbConnection db)
        {
            _db = db;
        }

        public Task CreateAsync(CustomUser user)
        {
            throw new NotImplementedException();
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
   ,AccessFailedCount = @AccessFailedCount
   ,LockoutEnd = @LockoutEnd
   ,TwoFactorEnabled = @TwoFactorEnabled
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
            var user = await _db.QueryAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE UserId = @userId",
                new { userId });
            return user.SingleOrDefault();
        }

        public async Task<CustomUser> FindByNameAsync(string userName)
        {
            var user = await _db.QueryAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE LoginName = @LoginName",
                new { LoginName = userName });
            return user.SingleOrDefault();
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
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(CustomUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(CustomUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(CustomUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<CustomUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
        public void Dispose() { }
    }
}