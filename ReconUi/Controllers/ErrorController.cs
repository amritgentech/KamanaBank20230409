using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReconUi.Controllers
{
    public class ErrorController : Controller
    {
        public virtual ActionResult UnauthorizedAccess()
        {
            return View();
        }
        public virtual ActionResult NotFound()
        {
            return View();
        }
        public virtual ActionResult InternalServerError()
        {
            return View();
        }
    }
}