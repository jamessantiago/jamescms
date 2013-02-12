using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using WebMatrix.WebData;

namespace jamescms.Models
{

    public interface IUsersContext
    {
        IDbSet<UserProfile> UserProfiles { get; set; }
    }

    public class UsersContext : DbContext, IUsersContext
    {
        public UsersContext()
            : base("AccountConnection")
        {
            
        }

        public virtual IDbSet<UserProfile> UserProfiles { get; set; }
        public virtual string[] Roles { get { return System.Web.Security.Roles.GetAllRoles(); } }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string[] IsInRole { get { return Roles.GetRolesForUser(UserName); } }

        public void AddToRole(string role)
        {
            Roles.AddUserToRole(UserName, role);
        }

        public DateTime CreateDate
        {
            get { return WebSecurity.GetCreateDate(UserName);}
        }
        public DateTime LastPasswordFailureDate
        {
            get { return WebSecurity.GetLastPasswordFailureDate(UserName); }
        }
        public DateTime PasswordChangedDate
        {
            get { return WebSecurity.GetPasswordChangedDate(UserName); }
        }
        public int PasswordFailuresSinceLastSuccess
        {
            get { return WebSecurity.GetPasswordFailuresSinceLastSuccess(UserName); }
        }
        public bool IsAccountLockedOut
        {
            get { return WebSecurity.IsAccountLockedOut(UserName, 10, 600); }
        }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
