using Db.IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Db.IdentityLibrary
{
    public class UserRepository<T> where T : IdentityUser
    {
        private readonly ReconContext _databaseContext;

        public UserRepository(ReconContext databaseContext)
        {
            _databaseContext = databaseContext;
            _databaseContext = new ReconContext();
        }

        internal T GeTByName(string userName)
        {
            var user = _databaseContext.AspNetUsers.SingleOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));
                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        internal T GeTByEmail(string email)
        {
            var user = _databaseContext.AspNetUsers.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));

                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        internal int Insert(T user)
        {
            _databaseContext.AspNetUsers.Add(new AspNetUsers
            {
                Id = user.Id,
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                AccessFailedCount = user.AccessFailedCount
            });

            return _databaseContext.SaveChanges();
        }

        /// <summary>
        /// Returns an T given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public T GeTById(string userId)
        {
            var user = _databaseContext.AspNetUsers.Find(userId);
            T result = (T)Activator.CreateInstance(typeof(T));

            result.Id = user.Id;
            result.UserName = user.UserName;
            result.PasswordHash = user.PasswordHash;
            result.SecurityStamp = user.SecurityStamp;
            result.Email = user.Email;
            result.EmailConfirmed = user.EmailConfirmed;
            result.PhoneNumber = user.PhoneNumber;
            result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            result.LockoutEnabled = user.LockoutEnabled;
            result.LockoutEndDateUtc = user.LockoutEndDateUtc;
            result.AccessFailedCount = user.AccessFailedCount;
            return result;
        }

        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            var user = _databaseContext.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            var passHash = user != null ? user.PasswordHash : null;
            return passHash;
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(T user)
        {
            var result = _databaseContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
            if (result != null)
            {
                result.UserName = user.UserName;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;

                try
                {
                    var roleName = user.Roles.FirstOrDefault().RoleId;
                    var roleId =
                        _databaseContext.AspNetRoles.Where(x => x.Name.Equals(roleName))
                            .Select(x => x.Id)
                            .FirstOrDefault();

                    //                    var resultData = _databaseContext.Database.SqlQuery<int>("exec dbo.updateUserRole @userId,@roleId",
                    //                        new SqlParameter("userId", user.Id),
                    //                        new SqlParameter("roleId", roleId));


                    var userRole = _databaseContext.AspNetUserRoles.Where(x => x.UserId == user.Id).First();
                    _databaseContext.AspNetUserRoles.Attach(userRole);
                    userRole.RoleId = roleId;

                    return _databaseContext.SaveChanges();
                }
                catch (Exception e)
                {
                    _databaseContext.AspNetUsers.Attach(result);
                    result.PasswordHash = user.PasswordHash;
                }
                _databaseContext.SaveChanges();

            }
            return 0;
        }
    }
}