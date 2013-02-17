using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace jamescms.Models
{
    public abstract class Entity
    {
        [Required]
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public static class ModelExtensions
    {
        public static string[] GetUserRoles(this UserProfile profile)
        {
            return Roles.GetRolesForUser(profile.UserName);
        }

        public static bool IsInRole(this UserProfile profile, string role)
        {
            return Roles.IsUserInRole(profile.UserName, role);
        }
    }
}