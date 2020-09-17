using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Globomantics.Framework.Models
{
    public partial class CustomUserStore : IUserLockoutStore<CustomUser, int>
    {
        public Task<DateTimeOffset> GetLockoutEndDateAsync(CustomUser user)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(CustomUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(CustomUser user)
        {
            user.AccessFailedCount += 1;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(CustomUser user)
        {
            user.LastLoginDate = DateTime.Now;
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(CustomUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(CustomUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(CustomUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(CustomUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
    }
}