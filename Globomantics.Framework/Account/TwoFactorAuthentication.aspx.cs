using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Globomantics.Framework.Account
{
    public partial class TwoFactorAuthentication : System.Web.UI.Page
    {
        public int RecoveryCodesLeft { get; set; }
        public bool IsMachineRemembered { get; set; }
        public bool HasAuthenticator { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            HasAuthenticator = manager.GetTwoFactorEnabled(User.Identity.GetUserId<int>());
            
        }
    }
}