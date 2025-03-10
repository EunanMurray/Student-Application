using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;

namespace StudentApplication.Pages.Admin
{
    public class ViewBudgetModel : PageModel
    {
        private readonly PrimaryContext _primaryContext;

        public ViewBudgetModel(PrimaryContext context)
        {
            _primaryContext = context;
        }

        public IList<Budget> Budget { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Budget = await _primaryContext.Budgets.ToListAsync();
        }
    }
}
