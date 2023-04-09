using Db.IdentityLibrary.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace Db.IdentityLibrary
{
    internal class UserRolesRepository
    {
        private readonly ReconContext _databaseContext;

        public UserRolesRepository(ReconContext database)
        {
            _databaseContext = database;
            _databaseContext = new ReconContext();
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IList<string> FindByUserId(string userId)
        {
            var roles = _databaseContext.AspNetUsers.
                Where(u => u.Id == userId).SelectMany(r => r.AspNetRoles);
            return roles.Select(r => r.Name).ToList();
        }
    }
}