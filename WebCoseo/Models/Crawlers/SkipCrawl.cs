using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Crawlers
{
    public class SkipCrawl
    {
        [Key]
        public int Id { get; set; }
        public string? Domain { get; set; }
        public string? Reason { get; set; }
        public DateTime? Created { get; set; }
    }
}
