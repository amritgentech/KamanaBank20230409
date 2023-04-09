using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReconUi.IdentityDAL;
using ReconUi.UiHelpers;

namespace ReconUi.Controllers
{
    [CustomAuthorize("Admin", "Super")]
    [Authorize]
    public class RoleController : BaseController
    {

        public RoleController()
        {
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(RoleDAL.GetRoles());
        }
        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            if (!string.IsNullOrEmpty(Role.Name))
            {
                RoleDAL.SaveRole(Role);
                try
                {
                    SaveActivityLogData("Role", "CreateRole", string.Format("Role name ={0}", Role.Name));
                }
                catch { }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("","Please Enter Name");
            return View(Role);
        }
        /// <summary>
        /// Update  a  role
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string roleId)
        {
            return View(RoleDAL.GetRoleByRoleId(roleId));
        }

        /// <summary>
        /// Update a Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(IdentityRole Role)
        {
            if (!string.IsNullOrEmpty(Role.Name))
            {
                RoleDAL.UpdateRole(Role);
                try
                {
                    SaveActivityLogData("Role", "UpdateRole", Role);
                }
                catch { }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Please Enter Name");
            return View(Role);
        }
        public ActionResult Delete(string roleId)
        {
            RoleDAL.DeleteRole(roleId);
            try
            {
                SaveActivityLogData("Role", "DeleteRole",roleId);
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}
