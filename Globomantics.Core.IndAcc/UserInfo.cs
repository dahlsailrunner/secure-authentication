using System.Collections.Generic;

namespace Globomantics.Core.IndAcc
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> UserClaims { get; set; }
    }
}
