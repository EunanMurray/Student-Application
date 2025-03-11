using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Secretary,Admin")]
    public class ViewBudgetModel : PageModel
    {
        private readonly PrimaryContext _primaryContext;

        public ViewBudgetModel(PrimaryContext context)
        {
            _primaryContext = context;
        }

        public Budget Budget { get; set; }
        public decimal TotalScholarshipAmount { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal FirstYearScholarships { get; set; }
        public decimal SecondYearScholarships { get; set; }
        public decimal ThirdYearScholarships { get; set; }
        public decimal FourthYearScholarships { get; set; }

        public async Task OnGetAsync()
        {
            var currentYear = DateTime.UtcNow.Year.ToString();
            Budget = await _primaryContext.Budgets
                .FirstOrDefaultAsync(b => b.BudgetYear == currentYear);

            if (Budget == null)
            {
                Budget = new Budget
                {
                    BudgetAmount = 80000,
                    BudgetUsage = 0,
                    BudgetYear = currentYear,
                    BudgetForFirstYears = 0,
                    BudgetForSecondYears = 0,
                    BudgetForThirdYears = 0,
                    BudgetForFourthYears = 0
                };
            }
            else
            {
                Budget.BudgetAmount = 80000;
            }

            var scholarships = await _primaryContext.Scholarships
                .Include(s => s.ScholarshipType)
                .Include(s => s.ScholarshipOfferHistories)
                    .ThenInclude(h => h.Applicant)
                .ToListAsync();

            var scholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                .Include(h => h.Applicant)
                .Include(h => h.Scholarship)
                    .ThenInclude(s => s.ScholarshipType)
                .Where(h => h.ResponseStatus == "Accepted" || h.ResponseStatus == "Pending")
                .ToListAsync();

            FirstYearScholarships = scholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 1)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            SecondYearScholarships = scholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 2)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            ThirdYearScholarships = scholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 3)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            FourthYearScholarships = scholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 4)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            Budget.BudgetForFirstYears = FirstYearScholarships;
            Budget.BudgetForSecondYears = SecondYearScholarships;
            Budget.BudgetForThirdYears = ThirdYearScholarships;
            Budget.BudgetForFourthYears = FourthYearScholarships;

            TotalScholarshipAmount = scholarships
                .Sum(s => s.ScholarshipType.PaymentAmount);

            RemainingBudget = Budget.BudgetAmount - TotalScholarshipAmount;
            Budget.BudgetUsage = TotalScholarshipAmount;

            _primaryContext.Budgets.Update(Budget);
            await _primaryContext.SaveChangesAsync();
        }
    }
}