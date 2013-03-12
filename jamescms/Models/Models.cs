using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using WebMatrix.WebData;

namespace jamescms.Models
{
    public abstract class Entity
    {
        [Required]
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public static class AccountModelExtensions
    {       

        public static string[] GetUserRoles(this UserProfile profile)
        {
            return Roles.GetRolesForUser(profile.UserName);
        }

        public static bool IsInRole(this UserProfile profile, string role)
        {
            return Roles.IsUserInRole(profile.UserName, role);
        }

        public static void AddToRole(this UserProfile profile, string role)
        {
            Roles.AddUserToRole(profile.UserName, role);
        }

        public static void RemoveFromRole(this UserProfile profile, string role)
        {
            Roles.RemoveUserFromRole(profile.UserName, role);
        }

        public static DateTime GetCreationDate(this UserProfile profile)
        {
            return WebSecurity.GetCreateDate(profile.UserName);
        }
        public static DateTime GetLastPasswordFailureDate(this UserProfile profile)
        {
            return WebSecurity.GetLastPasswordFailureDate(profile.UserName);
        }
        public static DateTime GetPasswordChangedDate(this UserProfile profile)
        {
            return WebSecurity.GetPasswordChangedDate(profile.UserName);
        }
        public static int GetPasswordFailuresSinceLastSuccess(this UserProfile profile)
        {
            return WebSecurity.GetPasswordFailuresSinceLastSuccess(profile.UserName);
        }

        public static bool GetAccountLockoutStatus(this UserProfile profile)
        {
            return WebSecurity.IsAccountLockedOut(profile.UserName, 10, 600);
        }
    }
    
}