using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace StudentApplication.Pages.Applicants
{
    [AllowAnonymous]
    public class ScholarshipConfirmationModel : PageModel
    {
        private readonly ILogger<ScholarshipConfirmationModel> _logger;

        public ScholarshipConfirmationModel(ILogger<ScholarshipConfirmationModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("ScholarshipConfirmation page accessed");
            _logger.LogInformation($"SuccessMessage in TempData: {TempData["SuccessMessage"]}");
            _logger.LogInformation($"InfoMessage in TempData: {TempData["InfoMessage"]}");
        }
    }
}