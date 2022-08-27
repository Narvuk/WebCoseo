using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Crawlers
{
    public class CrawledPages
    {
        [Key]
        public int Id { get; set; }
        public int CrawlSitesId { get; set; }
        public string? Url { get; set; }
        public string? Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastCrawled { get; set; }
        public DateTime? HtmlLastChecked { get; set; }
        public bool IsPriority { get; set; } = false;
    }
}
