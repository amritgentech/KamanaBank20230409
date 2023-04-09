using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Db;
using Db.IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReconUi.Models;

namespace ReconUi.UiHelpers
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        ReconContext context = new ReconContext();

        public CustomAuthorize(params string[] roles)
        {
            this.allowedroles = roles;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //                filterContext.Result = new RedirectResult("~/Account/Login");
                filterContext.HttpContext.Response.Redirect("~/Account/Login");
                base.OnAuthorization(filterContext);
                //                return;
            }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //            bool authorize = false;
            //            var user = httpContext.User.Identity;
            //            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //            var getRoles = UserManager.GetRoles(user.GetUserId());
            //            foreach (var role in allowedroles)
            //            {
            //                var isAuthorizedUser = getRoles.Any(x => x.Contains(role));
            //                if (isAuthorizedUser)
            //                {
            //                    authorize = true; /* return true if Entity has current user(active) with specific role */
            //                }
            //            }
            //            return authorize;
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Error", action = "UnauthorizedAccess" }));
            }
        }
    }
}