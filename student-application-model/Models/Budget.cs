using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApplicationModel.Models
{
    public class Budget
    {
        [Key]
        public int BudgetID { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetUsage { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OverBudgetAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetForFirstYears { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetForSecondYears { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetForThirdYears { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetForFourthYears { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetForPreOffers { get; set; }


        //What year this budget is for
        public string BudgetYear { get; set; }
    }
}
