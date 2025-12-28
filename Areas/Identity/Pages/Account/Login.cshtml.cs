// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ILdapService _ldapService;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger,
            ILdapService ldapService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _ldapService = ldapService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El usuario es requerido")]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "La contraseña es requerida")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Buscar el usuario en la base de datos
                var user = await _userManager.FindByNameAsync(Input.UserName);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                    return Page();
                }

                // Verificar si el usuario está activo
                if (!user.IsActive)
                {
                    _logger.LogWarning("Intento de inicio de sesión de usuario desactivado: {UserName}", Input.UserName);
                    ModelState.AddModelError(string.Empty, "Su cuenta ha sido desactivada. Contacte al administrador.");
                    return Page();
                }

                bool isAuthenticated = false;

                // Verificar si es usuario LDAP o local
                if (user.IsLdapUser)
                {
                    // Autenticar contra Active Directory
                    try
                    {
                        isAuthenticated = await _ldapService.ValidateCredentialsAsync(Input.UserName, Input.Password);
                        
                        if (isAuthenticated)
                        {
                            _logger.LogInformation("Usuario LDAP {UserName} autenticado correctamente", Input.UserName);
                        }
                        else
                        {
                            _logger.LogWarning("Fallo de autenticación LDAP para {UserName}", Input.UserName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al autenticar usuario LDAP {UserName}", Input.UserName);
                        ModelState.AddModelError(string.Empty, "Error al conectar con Active Directory. Intente más tarde.");
                        return Page();
                    }
                }
                else
                {
                    // Autenticar contra Identity (usuario local)
                    var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);
                    isAuthenticated = result.Succeeded;

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Usuario {UserName} bloqueado", Input.UserName);
                        return RedirectToPage("./Lockout");
                    }

                    if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "El usuario no tiene permitido iniciar sesión.");
                        return Page();
                    }
                }

                if (isAuthenticated)
                {
                    // Iniciar sesión en Identity
                    await _signInManager.SignInAsync(user, Input.RememberMe);
                    _logger.LogInformation("Usuario {UserName} ha iniciado sesión ({AuthType})", 
                        Input.UserName, 
                        user.IsLdapUser ? "LDAP" : "Local");
                    
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
