using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using StudentApplication.Services;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace StudentApplication.Pages.Account
{
    public class ApplicantRegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ApplicantRegisterModel> _logger;

        public ApplicantRegisterModel(
            UserManager<IdentityUser> userManager,
            IEmailService emailService,
            ILogger<ApplicantRegisterModel> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Add user to Applicant role
                    await _userManager.AddToRoleAsync(user, "Applicant");

                    try
                    {
                        // Generate the email confirmation token
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // Encode the token
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // Generate the callback URL
                        var callbackUrl = Url.Page(
                            "/Applications/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme
                        );

                        if (string.IsNullOrEmpty(callbackUrl))
                        {
                            throw new InvalidOperationException("Failed to generate confirmation URL.");
                        }

                        // Log the callback URL for debugging
                        _logger.LogInformation($"Generated callback URL: {callbackUrl}");

                        await _emailService.SendVerificationEmailAsync(Input.Email, callbackUrl);

                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send verification email");

                        // Delete the user if email sending fails
                        await _userManager.DeleteAsync(user);

                        ModelState.AddModelError(string.Empty,
                            "Error sending verification email. Please try again later.");
                        return Page();
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}