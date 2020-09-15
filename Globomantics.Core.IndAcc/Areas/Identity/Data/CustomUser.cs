﻿using System;
using Microsoft.AspNetCore.Identity;

namespace Globomantics.Core.IndAcc.Areas.Identity.Data
{
    public class CustomUser : IdentityUser<int>
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string PasswordSalt { get; set; }
        public short Status { get; set; }
        public DateTime PasswordModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreateDate { get; set; }

        public override int Id => UserId;
        public override string NormalizedEmail => LoginName.ToUpperInvariant();
        public override string Email => LoginName;
        public override string UserName => LoginName;
        public override bool LockoutEnabled => true;
    }
}
