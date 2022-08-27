using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Reports.MySites.Dashboard
{
    public class MySitesPages_Dash_Monthly_Total
    {
        [Key]
        public int Id { get; set; }
        public string? MonthYear { get; set; }
        public int? DtMonth { get; set; }
        public int? DtYear { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int? Amount { get; set; }
    }
}