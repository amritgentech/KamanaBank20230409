using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Db;
using Helper.GlobalHelpers;
using Microsoft.AspNet.Identity;

namespace ReconUi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var owinContext = HttpContext.Current.Request.GetOwinContext();
                //                var authenticationTypes = owinContext.Authentication.GetAuthenticationTypes();
                //                owinContext.Authentication.SignOut(authenticationTypes.Select(o => o.AuthenticationType).ToArray());
                owinContext.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                //                Session.Abandon();
            }
            catch (Exception ex)
            {
            }

            GlobalHelper.ErrorLogging(Server.GetLastError());
            //send error to mail..
            var log = ApplicationErrorLog.error(Server.GetLastError());
            //Mail.SendMail("Recon App error", log + "---" + Server.GetLastError().Message);

            //redirect to error page..

            HttpContext ctx = HttpContext.Current;
            ctx.Response.Clear();
            RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            rc.RouteData.Values["action"] = "InternalServerError";

            // TODO: distinguish between 404 and other errors if needed

            rc.RouteData.Values["controller"] = "Error";
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(rc, "Error");
            controller.Execute(rc);
            ctx.Server.ClearError();
        }
    }
}
