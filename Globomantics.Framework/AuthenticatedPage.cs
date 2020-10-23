using System;
using System.Web.Security;
using System.Web.UI;

namespace Globomantics.Framework
{
    public class AuthenticatedPage : Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            base.OnPreLoad(e);
        }
    }
}
