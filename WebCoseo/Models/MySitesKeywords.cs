using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    internal class MySitesKeywords
    {
        [Key]
        public int Id { get; set; }
        public int MySiteId { get; set; }
        public string? Keyword { get; set; }
        public string? Url { get; set; }
        public int? Clicks { get; set; } = 0;
        // Get Counts
        public int? Impressions { get; set; } = 0;
        public double? ctr { get; set; } = 0.0;
        public double? AvgPositions { get; set; } = 0.0;
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
