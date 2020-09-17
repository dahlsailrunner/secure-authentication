using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Globomantics.Framework.Models
{
    public partial class CustomUserStore : IUserTwoFactorStore<CustomUser, int>
    {
        public Task SetTwoFactorEnabledAsync(CustomUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(CustomUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}
