// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebApp.Enums;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ILdapService _ldapService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ILdapService ldapService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _ldapService = ldapService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El usuario es requerido")]
            [DataType(DataType.Text)]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "El nombre es requerido")]
            [Display(Name = "Nombre")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "El apellido es requerido")]
            [Display(Name = "Apellido")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "El email es requerido")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Usuario de Active Directory")]
            public bool IsLdapUser { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// Handler para buscar un usuario en LDAP y autocompletar el formulario
        /// </summary>
        public async Task<IActionResult> OnPostSearchLdapAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (string.IsNullOrWhiteSpace(Input.UserName))
            {
                StatusMessage = "Error: Ingrese un nombre de usuario para buscar";
                return Page();
            }

            try
            {
                // Verificar si el usuario ya existe en el sistema
                var existingUser = await _userManager.FindByNameAsync(Input.UserName);
                if (existingUser != null)
                {
                    StatusMessage = $"Error: El usuario '{Input.UserName}' ya existe en el sistema";
                    return Page();
                }

                // Buscar el usuario en Active Directory
                var ldapUser = await _ldapService.GetUserByUsernameAsync(Input.UserName);

                if (ldapUser == null)
                {
                    StatusMessage = $"No se encontró el usuario '{Input.UserName}' en Active Directory";
                    return Page();
                }

                // Autocompletar los campos del formulario
                Input.FirstName = ldapUser.FirstName;
                Input.LastName = ldapUser.LastName;
                Input.Email = !string.IsNullOrEmpty(ldapUser.Email) 
                    ? ldapUser.Email 
                    : $"{ldapUser.UserName}@ldap.local";
                Input.IsLdapUser = true;

                StatusMessage = $"Usuario '{ldapUser.DisplayName ?? ldapUser.UserName}' encontrado en Active Directory. Verifique los datos y presione Registrar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuario {Username} en LDAP", Input.UserName);
                StatusMessage = "Error: No se pudo conectar con Active Directory";
            }

            ModelState.Clear();
            return Page();
        }

        /// <summary>
        /// Handler para registrar el usuario (local o LDAP)
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                var existingUser = await _userManager.FindByNameAsync(Input.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, $"El usuario '{Input.UserName}' ya existe en el sistema");
                    return Page();
                }

                // Si es usuario LDAP, verificar que existe en AD
                if (Input.IsLdapUser)
                {
                    try
                    {
                        var ldapUser = await _ldapService.GetUserByUsernameAsync(Input.UserName);
                        if (ldapUser == null)
                        {
                            ModelState.AddModelError(string.Empty, $"El usuario '{Input.UserName}' no existe en Active Directory");
                            return Page();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al verificar usuario LDAP {Username}", Input.UserName);
                        ModelState.AddModelError(string.Empty, "Error al conectar con Active Directory");
                        return Page();
                    }
                }

                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    IsLdapUser = Input.IsLdapUser,
                    EmailConfirmed = Input.IsLdapUser // Usuarios LDAP ya están verificados
                };

                // Contraseña: temporal para locales, aleatoria para LDAP
                var password = Input.IsLdapUser ? GenerateRandomPassword() : "Temporal.1";
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Username} creado ({AuthType})", 
                        Input.UserName, 
                        Input.IsLdapUser ? "LDAP" : "Local");

                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());

                    if (Input.IsLdapUser)
                    {
                        StatusMessage = $"Usuario '{Input.UserName}' registrado desde Active Directory. Se autenticará con sus credenciales del dominio.";
                        return RedirectToPage();
                    }
                    else
                    {
                        // Usuario local - enviar email de confirmación
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        StatusMessage = "Usuario local creado con contraseña Temporal.1";
                        return RedirectToPage();
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        /// <summary>
        /// Genera una contraseña aleatoria segura para usuarios LDAP
        /// </summary>
        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%&*";
            var random = new Random();
            var password = new char[16];
            
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            
            return $"Ld@p{new string(password)}1!";
        }
    }
}
