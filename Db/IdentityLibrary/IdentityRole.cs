﻿namespace Db.IdentityLibrary
{
    using Microsoft.AspNet.Identity;
    using System;

    /// <summary>
    /// Class that implements the ASP.NET Identity
    /// IRole interface
    /// </summary>
    public class IdentityRole : IRole
    {
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string name)
            : this()
        {
            Name = name;
        }

        public IdentityRole(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}