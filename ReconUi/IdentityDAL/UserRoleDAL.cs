using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Db;
using Db.Model;
using Db.IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ReconUi.Models;

namespace ReconUi.IdentityDAL
{
    public class UserRoleDAL
    {
        public IdentityRelatedModel GetUserRoleData()
        {
            using (var context = new ReconContext())
            {
                var Users = context.AspNetUsers.Where(x => !x.UserName.ToLower().Equals("super")).ToList();
                var UserRoles = context.AspNetUserRoles.ToList();

                var userNotHaveRole = (from a in Users where !(from b in UserRoles select b.UserId).Contains(a.Id) select a);

                List<IdentityRelatedModel> lstModel = new List<IdentityRelatedModel>();

                foreach (var user in userNotHaveRole)
                {

                    IdentityRelatedModel model = new IdentityRelatedModel();
                    model.UserId = user.Id;
                    model.UserName = user.UserName;
                    model.UserRoleId = "";
                    lstModel.Add(model);
                }

                AllroleWithAllUser _alluserWithRole = new AllroleWithAllUser();
                _alluserWithRole.AllDetailsUserlist = GetUserNameResepectiveToRole();

                foreach (var ob in _alluserWithRole.AllDetailsUserlist)
                {
                    foreach (var role in ob.UserRoleNamelst)
                    {
                        IdentityRelatedModel model = new IdentityRelatedModel();
                        model.UserId = ob.userId;
                        model.UserName = ob.UserName;
                        model.UserRoleId = role;
                        //                model.UserRoleName = GetUserRolesByUserId(ob.userId).FirstOrDefault();
                        model.UserRoleId = context.AspNetRoles
                            .Where(x => x.Name.Equals(role)).Select(x => x.Id)
                            .FirstOrDefault();
                        lstModel.Add(model);
                    }
                }
                _alluserWithRole.AllDetailsUserlist = GetUserNameResepectiveToRole();
                lstModel = lstModel.OrderBy(x => x.UserName).ToList();

                var result = new IdentityRelatedModel()
                {
                    IdentityRelatedModels = lstModel,
                    UserRolesList = GetAll_UserRoles()
                };
                return result;
            }
        }
        public List<AllroleWithAllUser> GetUserNameResepectiveToRole()
        {
            using (var context = new ReconContext())
            {
                var users = context.AspNetUsers.Where(x => x.UserName.ToLower() != "super").ToList();
                List<AllroleWithAllUser> lstAllroleWithAllUsers = new List<AllroleWithAllUser>();
                foreach (var user in users)
                {
                    AllroleWithAllUser allroleWithAllUser = new AllroleWithAllUser();
                    var roles = GetUserRolesByUserId(user.Id);
                    allroleWithAllUser.UserName = user.UserName;
                    allroleWithAllUser.userId = user.Id;
                    allroleWithAllUser.UserRoleNamelst = roles;
                    lstAllroleWithAllUsers.Add(allroleWithAllUser);
                }
                return lstAllroleWithAllUsers;
            }
        }
        public List<string> GetUserRolesByUserId(string userId)
        {
            using (var context = new ReconContext())
            {
                var UserManager = new UserManager<ApplicationUser>(new Db.IdentityLibrary.UserStore<ApplicationUser>(context));
                var roleStore = new RoleStore<IdentityRole>(context);
                var RoleManager = new RoleManager<IdentityRole>(roleStore);
                List<string> ListOfRoleNames = new List<string>();

                var roles = context.AspNetUserRoles.ToList();
                var allowedroles = roles.Where(x => x.UserId.Equals(userId)).ToList();

                foreach (var role in allowedroles)
                {
                    var rolename = context.AspNetRoles.Where(x => x.Id == role.RoleId).Select(x => x.Name).FirstOrDefault();
                    ListOfRoleNames.Add(rolename);
                }
                return ListOfRoleNames;
            }
        }
        public List<SelectListItem> GetAll_UserRoles()
        {
            List<SelectListItem> listrole = new List<SelectListItem>();
            listrole.Add(new SelectListItem
            {
                Text = "select",
                Value = "0"
            });
            using (ReconContext db = new ReconContext())
            {
                foreach (var item in db.AspNetRoles.Where(x => !x.Name.ToLower().Contains("systemadmin")))
                {
                    listrole.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = Convert.ToString(item.Id)
                    });
                }
            }
            return listrole;
        }

        public object GetUserByUserId(string userId)
        {
            using (var context = new ReconContext())
            {
                return context.AspNetUsers.Where(x => x.Id.Equals(userId)).FirstOrDefault();
            }
        }
        public string GetRoleNameByRoleID(string RoleId)
        {
            using (var context = new ReconContext())
            {
                var roleName =
                    (from UP in context.AspNetRoles where UP.Id == RoleId.ToString() select UP.Name).SingleOrDefault();
                return roleName;
            }

        }
        public void UpdateUser(string userId, string userName)
        {
            using (var context = new ReconContext())
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                   .GetUserManager<ApplicationUserManager>().FindById(userId);
                user.UserName = userName;
                context.AspNetUsers.AddOrUpdate(new AspNetUsers() { Id = userId, UserName = userName });
                context.SaveChanges();
            }
        }
        public void UpdateUser(ApplicationUser User)
        {
            using (var context = new ReconContext())
            {
                context.AspNetUsers.AddOrUpdate(new AspNetUsers() { Id = User.Id, UserName = User.UserName });
                context.SaveChanges();
            }
        }
        public void DeleteUser(string userId)
        {
            using (var context = new ReconContext())
            {
                //remove user..
                var user = context.AspNetUsers.Where(x => x.Id.Equals(userId)).FirstOrDefault();
                context.AspNetUsers.Remove(user);

                //remove userrole..
                var isExist = context.AspNetUserRoles.Any(x => x.UserId.Equals(userId));
                if (isExist)
                {
                    var userRole =
                        context.AspNetUserRoles.Where(x => x.UserId.Equals(userId))
                            .FirstOrDefault();
                    RemoveRoleFromUser(userId, userRole.RoleId);  //remove old userrole  if exist and add new role to the user ..
                }
                context.SaveChanges();
            }
        }
        public void AddRoleToUser(string UserId, string UserRoleName)
        {
            using (var context = new ReconContext())
            {
                var usermanager = new UserManager<ApplicationUser>(new Db.IdentityLibrary.UserStore<ApplicationUser>(context));
                usermanager.AddToRole(UserId, UserRoleName);
            }
        }
        public void RemoveRoleFromUser(string userId, string roleId)
        {
            using (var context = new ReconContext())
            {
                if (!string.IsNullOrEmpty(roleId))
                {
                    var userRole =
                       context.AspNetUserRoles.Where(x => x.RoleId.Equals(roleId) && x.UserId.Equals(userId))
                           .FirstOrDefault();
                    context.AspNetUserRoles.Remove(userRole);
                    context.SaveChanges();
                }
            }
        }

        public void AddUpdateUserRole(string userId, string roleId)
        {
            using (var context = new ReconContext())
            {
                var isExist = context.AspNetUserRoles.Any(x => x.UserId.Equals(userId));
                if (isExist)
                {
                    var userRole =
                        context.AspNetUserRoles.Where(x => x.UserId.Equals(userId))
                            .FirstOrDefault();
                    RemoveRoleFromUser(userId, userRole.RoleId);  //remove old userrole  if exist and add new role to the user ..
                }

                context.AspNetUserRoles.Add(new AspNetUserRoles()
                {
                    RoleId = roleId,
                    UserId = userId
                });

                context.SaveChanges();
            }
        }
    }
}
