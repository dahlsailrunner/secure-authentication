using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Globomantics.Core.IndAcc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Globomantics.Core.IndAcc.Areas.Identity
{
    public partial class CustomUserStore : IUserTwoFactorStore<CustomUser>,
                                           IUserAuthenticationTokenStore<CustomUser>
    {
        public Task SetTwoFactorEnabledAsync(CustomUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task SetTokenAsync(CustomUser user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTokenAsync(CustomUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTokenAsync(CustomUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
