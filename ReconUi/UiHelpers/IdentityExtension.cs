using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace ReconUi.UiHelpers
{
    public static class IdentityExtension
    {
        public static string IsAdmin(this IIdentity identity)
        {
            if (identity == null)
                return null;

            var IsAdmin = (identity as ClaimsIdentity).FindFirst("IsAdmin");
            return string.Format("{0}", IsAdmin).Trim();
        }
    }
}