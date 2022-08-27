using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class MySites
    {
        [Key]
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        // Get Counts
        public int? PagesCount { get; set; } = 0;
        public int? BacklinksCount { get; set; } = 0;
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

    }
}
