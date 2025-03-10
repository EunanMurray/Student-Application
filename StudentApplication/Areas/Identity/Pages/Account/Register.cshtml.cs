// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using StudentApplication.Services;

namespace StudentApplicationPages.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailService;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    try
                    {
                        // Generate a confirmation code for the email
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        _logger.LogInformation("Email confirmation token generated");

                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // Create the confirmation URL
                        var callbackUrl = Url.Page(
                            "/Applications/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        if (string.IsNullOrEmpty(callbackUrl))
                        {
                            _logger.LogError("Failed to generate callback URL");
                            throw new InvalidOperationException("Failed to generate confirmation URL.");
                        }

                        _logger.LogInformation($"Callback URL: {callbackUrl}");

                        // Send the verification email
                        var emailSent = await _emailService.SendVerificationEmailAsync(Input.Email, callbackUrl);

                        if (!emailSent)
                        {
                            _logger.LogError($"Failed to send verification email: {_emailService.GetLastError()}");

                            // Delete the user if email sending fails
                            await _userManager.DeleteAsync(user);

                            ModelState.AddModelError(string.Empty,
                                $"Error sending verification email: {_emailService.GetLastError()}. Please try again later.");
                            return Page();
                        }

                        _logger.LogInformation("Verification email sent successfully");

                        // Redirect to confirmation page
                        return RedirectToPage("/Applications/RegisterConfirmation", new { email = Input.Email });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in email verification process");

                        // Delete the user if there was an error
                        await _userManager.DeleteAsync(user);

                        ModelState.AddModelError(string.Empty, $"An error occurred during registration: {ex.Message}");
                        return Page();
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed; redisplay the form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}