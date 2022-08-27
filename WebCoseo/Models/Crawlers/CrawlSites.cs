using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Crawlers
{
    public class CrawlSites
    {
        [Key]
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastCrawled { get; set; }
    }
}
