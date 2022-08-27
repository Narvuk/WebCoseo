using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Crawlers
{
    public class CrawlPool
    {
        [Key]
        public int Id { get; set; }
        public int? CrawlSitesId { get; set; }
        public int? CrawledPagesId { get; set; }
        public string? Url { get; set; }
        public string? Status { get; set; }
        public bool IsPriority { get; set; } = false;
        public DateTime? Created { get; set; }
    }
}
