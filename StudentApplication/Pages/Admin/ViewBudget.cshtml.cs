using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
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
        public decimal PreviousYearFourthYearScholarships { get; set; }
        public string SelectedYear { get; set; }
        public List<Applicant> FirstYearApplicants { get; set; }
        public List<Applicant> SecondYearApplicants { get; set; }
        public List<Applicant> ThirdYearApplicants { get; set; }
        public List<Applicant> FourthYearApplicants { get; set; }
        public List<ScholarshipOfferHistory> ScholarshipOffers { get; set; } 


        public async Task OnGetAsync(string year = null)
        {
            SelectedYear = year ?? DateTime.UtcNow.Year.ToString();

            Budget = await _primaryContext.Budgets
                .FirstOrDefaultAsync(b => b.BudgetYear == SelectedYear);

            if (Budget == null)
            {
                Budget = new Budget
                {
                    BudgetAmount = 80000,
                    BudgetUsage = 0,
                    BudgetYear = SelectedYear,
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

             ScholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                .Include(h => h.Applicant)
                .Include(h => h.Scholarship)
                    .ThenInclude(s => s.ScholarshipType)
                .Where(h => (h.ResponseStatus == "Accepted" || h.ResponseStatus == "Pending") && h.OfferDate.Year.ToString() == SelectedYear)
                .ToListAsync();

            FirstYearScholarships = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 1)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            SecondYearScholarships = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 2)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            ThirdYearScholarships = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 3)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            FourthYearScholarships = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 4)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            FirstYearApplicants = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 1)
                .Select(h => h.Applicant)
                .Distinct()
                .ToList();

            SecondYearApplicants = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 2)
                .Select(h => h.Applicant)
                .Distinct()
                .ToList();

            ThirdYearApplicants = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 3)
                .Select(h => h.Applicant)
                .Distinct()
                .ToList();

            FourthYearApplicants = ScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 4)
                .Select(h => h.Applicant)
                .Distinct()
                .ToList();


            var previousYear = (int.Parse(SelectedYear) - 1).ToString();
            var previousYearScholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                .Include(h => h.Applicant)
                .Include(h => h.Scholarship)
                    .ThenInclude(s => s.ScholarshipType)
                .Where(h => (h.ResponseStatus == "Accepted" || h.ResponseStatus == "Pending") && h.OfferDate.Year.ToString() == previousYear)
                .ToListAsync();

            PreviousYearFourthYearScholarships = previousYearScholarshipOffers
                .Where(h => h.Applicant?.CollegeYear == 4)
                .Sum(h => h.Scholarship?.ScholarshipType?.PaymentAmount ?? 0);

            Budget.BudgetForFirstYears = FirstYearScholarships;
            Budget.BudgetForSecondYears = SecondYearScholarships;
            Budget.BudgetForThirdYears = ThirdYearScholarships;
            Budget.BudgetForFourthYears = FourthYearScholarships;

            TotalScholarshipAmount = FirstYearScholarships + SecondYearScholarships + ThirdYearScholarships + FourthYearScholarships;

            RemainingBudget = Budget.BudgetAmount - TotalScholarshipAmount;
            Budget.BudgetUsage = TotalScholarshipAmount;

            _primaryContext.Budgets.Update(Budget);
            await _primaryContext.SaveChangesAsync();
        }
    }
}