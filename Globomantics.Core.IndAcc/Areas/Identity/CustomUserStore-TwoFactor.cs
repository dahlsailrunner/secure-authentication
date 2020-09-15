using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Globomantics.Core.IndAcc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Globomantics.Core.IndAcc.Areas.Identity
{
    public partial class CustomUserStore : IUserTwoFactorStore<CustomUser>,
                                           IUserAuthenticatorKeyStore<CustomUser>,
                                           IUserTwoFactorRecoveryCodeStore<CustomUser>
    {
        private const string AuthenticatorKeyName = "AuthenticatorKey";
        private const string RecoveryCodesName = "RecoveryCodes";

        public Task SetTwoFactorEnabledAsync(CustomUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetAuthenticatorKeyAsync(CustomUser user, string key, CancellationToken cancellationToken)
        {
            return _db.ExecuteAsync("INSERT INTO dbo.UserToken VALUES (@UserId, @Provider, @Name, @Value)", 
                new {user.UserId, Provider = "Globomantics", Name = AuthenticatorKeyName, Value = key});
        }

        public Task<string> GetAuthenticatorKeyAsync(CustomUser user, CancellationToken cancellationToken)
        {
            if (_db.State != ConnectionState.Open)
            {
                _db.Open();
            }
            return _db.QuerySingleOrDefaultAsync<string>(
                "SELECT Value FROM dbo.UserToken WHERE UserId = @UserId AND Name = @AuthenticatorKeyName",
                new { user.UserId, AuthenticatorKeyName });
        }

        public Task ReplaceCodesAsync(CustomUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RedeemCodeAsync(CustomUser user, string code, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountCodesAsync(CustomUser user, CancellationToken cancellationToken)
        {
            var x = await _db.ExecuteScalarAsync<string>(
                "SELECT Value FROM dbo.UserToken WHERE UserId = @UserId AND Name = @RecoveryCodesName",
                new { user.UserId, RecoveryCodesName });

            return (string.IsNullOrEmpty(x)) ? 0 : x.Split(";").Count();
        }
    }
}
