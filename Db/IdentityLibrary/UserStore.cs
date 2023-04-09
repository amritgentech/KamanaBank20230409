using Db.IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Db.IdentityLibrary
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<T> : IUserRoleStore<T>,
                                IUserStore<T>,
                                IUserPasswordStore<T>,
                                IUserEmailStore<T>,
                                IUserLockoutStore<T, string>,
                                IUserTwoFactorStore<T, string>
    where T : IdentityUser
    {
        private readonly UserRepository<T> _userTable;
        private readonly UserRolesRepository _userRolesTable;

        public UserStore(ReconContext databaseContext)
        {
            _userTable = new UserRepository<T>(databaseContext);
            _userRolesTable = new UserRolesRepository(databaseContext);
        }

        public Task CreateAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.Run(() => _userTable.Insert(user));
        }

        public Task<T> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return Task.Run(() => _userTable.GeTById(userId));
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<T> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            return Task.Run(() => _userTable.GeTByName(userName));
        }

        public Task<IList<string>> GetRolesAsync(T user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.Run(() => _userRolesTable.FindByUserId(user.Id));
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.Run(() => _userTable.GetPasswordHash(user.Id));
        }

        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            return Task.Run(() => user.PasswordHash = passwordHash);
        }

        public Task<T> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return Task.Run(() => _userTable.GeTByEmail(email));
        }

        public Task<string> GetEmailAsync(T user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<int> GetAccessFailedCountAsync(T user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            return
                 Task.FromResult(user.LockoutEndDateUtc.HasValue
                     ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                     : new DateTimeOffset());
        }

        public Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;

            return Task.Run(() => _userTable.Update(user));
        }

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(T user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user)
        {
            //            var store = new UserStore<T>(new ReconContext());
            //            var manager = new UserManager<T>(store);
            //            manager.Create(user);
            //            foreach (var role in user.Roles)
            //            {
            //                var roleName =
            //                    new ReconContext().AspNetRoles.Where(x => x.Id == role.RoleId).Select(x => x.Name).FirstOrDefault();
            //                manager.AddToRole(user.Id, roleName);
            //            }
            return Task.Run(() => _userTable.Update(user));
        }

        public Task AddToRoleAsync(T user, string roleName)
        {
           return Task.Run(() => user.Roles.Add(new IdentityUserRole() { RoleId = roleName, UserId = user.Id }));
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}