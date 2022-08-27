using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Reports.Backlinks.Websites
{
    public class BL_Websites_Monthly_Total
    {
        [Key]
        public int Id { get; set; }
        public int? WebsiteSiteId { get; set; }
        public string? MonthYear { get; set; }
        public int? DtMonth { get; set; }
        public int? DtYear { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int? Amount { get; set; }
    }
}
