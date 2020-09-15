using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Globomantics.Core.IndAcc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Globomantics.Core.IndAcc.Areas.Identity
{
    public partial class CustomUserStore : IUserPasswordStore<CustomUser>
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
   ,AccessFailedCount = @AccessFailedCount
   ,LockoutEnd = @LockoutEnd,
   ,TwoFactorEnabled = @TwoFactorEnabled
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
            var user = await _db.QueryAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE UserId = @userId", 
                new { userId } );
            return user.SingleOrDefault();
        }

        public async Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _db.QueryAsync<CustomUser>("SELECT * FROM GlobomanticsUser WHERE LoginName = @LoginName",
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

        public void Dispose() { }
    }
}
