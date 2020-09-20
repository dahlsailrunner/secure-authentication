using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Globomantics.Core.Identity
{
    public class CustomUserStore : IUserPasswordStore<CustomUser>, IUserEmailStore<CustomUser>
    {
        private readonly IDbConnection _db;

        public CustomUserStore(IDbConnection db)
        {
            _db = db;
        }

        public Task<string> GetUserIdAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LoginName);
        }

        public Task SetUserNameAsync(CustomUser user, string userName, CancellationToken cancellationToken)
        {
            user.LoginName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LoginName.ToUpper());
        }

        public Task SetNormalizedUserNameAsync(CustomUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                _db.ExecuteAsync(
                    @"
UPDATE GlobomanticsUser 
SET PasswordHash = @PasswordHash 
   ,PasswordSalt = @PasswordSalt
   ,LoginName = @LoginName
   ,PasswordModifiedDate = @PasswordModifiedDate
   ,LastLoginDate = @LastLoginDate
   ,CreateDate = @CreateDate
   ,Status = @Status
WHERE UserId = @UserId",
                    user);
                result = IdentityResult.Success;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating identity.");
                result = IdentityResult.Failed();
            }
            return Task.FromResult(result);
        }

        public Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _db.QueryAsync<CustomUser>(
                "SELECT * FROM GlobomanticsUser WHERE UserId = @userId",
                new { userId });
            return user.SingleOrDefault();
        }

        public async Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _db.QueryAsync<CustomUser>(
                "SELECT * FROM GlobomanticsUser WHERE LoginName = @LoginName",
                new { LoginName = normalizedUserName });
            return user.SingleOrDefault();
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            user.PasswordSalt = null;
            user.PasswordModifiedDate = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(CustomUser user, string email, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(CustomUser user, CancellationToken cancellationToken)
        {
            //TODO: implement this
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(CustomUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CustomUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return FindByNameAsync(normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(CustomUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}
