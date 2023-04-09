using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ReconUi;
using ReconUi.IdentityDAL;
using ReconUi.Models;
using ReconUi.UiHelpers;

//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

namespace ReconUi.Controllers
{
    [CustomAuthorize("Admin", "Super")]
    [Authorize]
    public class UserController : BaseController
    {
        private UserRoleDAL userRoleDal;
        public UserController()
        {
            userRoleDal = new UserRoleDAL();
        }

        #region UserRole
        public ActionResult Index()
        {
            return View(userRoleDal.GetUserRoleData());
        }
        public ActionResult Update(string userId)
        {
            return View(userRoleDal.GetUserByUserId(userId));
        }
        [HttpPost]
        public ActionResult UpdateUserRole(FormData formData)
        {
            try
            {
                //update user..
                //                userRoleDal.UpdateUser(formData.UserId, formData.UserName);

                if (string.IsNullOrEmpty(formData.UserRoleId) || formData.UserRoleId.Equals("0"))
                {
                    throw new ArgumentException("Please select Role for the user..");
                }
                var UserRoleName = userRoleDal.GetRoleNameByRoleID(formData.UserRoleId);

                //add role to user..
                //                userRoleDal.AddRoleToUser(formData.UserId, UserRoleName);
                //                userRoleDal.AddUpdateUserRole(formData.UserId, UserRoleName);
                userRoleDal.AddUpdateUserRole(formData.UserId, formData.UserRoleId);
                try
                {
                    SaveActivityLogData("User", "UpdateUserRole", UserRoleName);
                }
                catch { }
                return PartialView("_UserRoleTable", userRoleDal.GetUserRoleData());
            }

            catch (ArgumentException ex)
            {
                TempData["notice"] = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                TempData["notice"] = "Some error eccurred";
            }
            return PartialView("_UserRoleTable", userRoleDal.GetUserRoleData());
        }
        public ActionResult DeleteUserRole(string userId, string roleId)
        {
            userRoleDal.RemoveRoleFromUser(userId, roleId);
            try
            {
                SaveActivityLogData("User", "DeleteUserRole", string.Format("User id ={0} Role id={1}", userId, roleId));
            }
            catch { }
            return RedirectToAction("Index", "User");
        }
        #endregion
        #region User
        public ActionResult Create()
        {
            return RedirectToAction("Register", "Account");
        }
        [HttpPost]
        public ActionResult Update(ApplicationUser User)
        {
            userRoleDal.UpdateUser(User);
            try
            {
                SaveActivityLogData("User", "UpdateUser", User);
            }
            catch { }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string userId)
        {
            userRoleDal.DeleteUser(userId);
            try
            {
                SaveActivityLogData("User", "DeleteUser", string.Format("User id ={0}", userId));
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

    }
}
