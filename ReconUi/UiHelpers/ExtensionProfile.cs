using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;

namespace ReconUi.UiHelpers
{
    public class ExtensionProfile
    {
        public static Guid GetUserGuid(IIdentity identity)
        {
            return new Guid(HttpContext.Current.User.Identity.GetUserId());
        }
    }
}