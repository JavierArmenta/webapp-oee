using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el usuario se autentica mediante LDAP/Active Directory
        /// </summary>
        [Display(Name = "Usuario LDAP")]
        public bool IsLdapUser { get; set; } = false;

        /// <summary>
        /// Indica si el usuario está activo (false = borrado lógico)
        /// </summary>
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Tipo de autenticación como texto
        /// </summary>
        public string AuthenticationType => IsLdapUser ? "Active Directory" : "Local";
    }
}
