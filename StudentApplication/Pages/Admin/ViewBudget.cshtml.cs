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
    [Authorize(Roles = "Secretary")]
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

            Budget.BudgetAmount = 80000;

            if (Budget == null)
            {
                Budget = new Budget { BudgetAmount = 80000, BudgetUsage = 0, BudgetYear = currentYear, BudgetForFirstYears = 0, BudgetForSecondYears = 0, BudgetForThirdYears = 0, BudgetForFourthYears = 0};
            }

            var scholarships = await _primaryContext.Scholarships
                .Include(s => s.ScholarshipType)
                .Include(s => s.Applicants)
                .ToListAsync();

            
            FirstYearScholarships = scholarships
                .Where(s => s.Applicants.Any(a => a.CollegeYear == 1))
                .Sum(s => s.ScholarshipType.PaymentAmount);

            SecondYearScholarships = scholarships
                .Where(s => s.Applicants.Any(a => a.CollegeYear == 2))
                .Sum(s => s.ScholarshipType.PaymentAmount);

            ThirdYearScholarships = scholarships
                .Where(s => s.Applicants.Any(a => a.CollegeYear == 3))
                .Sum(s => s.ScholarshipType.PaymentAmount);

            FourthYearScholarships = scholarships
                .Where(s => s.Applicants.Any(a => a.CollegeYear == 4))
                .Sum(s => s.ScholarshipType.PaymentAmount);

            
            Budget.BudgetForFirstYears += FirstYearScholarships;
            Budget.BudgetForSecondYears += SecondYearScholarships;
            Budget.BudgetForThirdYears += ThirdYearScholarships;
            Budget.BudgetForFourthYears += FourthYearScholarships;

            TotalScholarshipAmount = scholarships
                .Sum(s => s.ScholarshipType.PaymentAmount);

            RemainingBudget = Budget.BudgetAmount - TotalScholarshipAmount;

            Budget.BudgetUsage = TotalScholarshipAmount;

            _primaryContext.Budgets.Update(Budget);
            await _primaryContext.SaveChangesAsync();
        }
    }
}