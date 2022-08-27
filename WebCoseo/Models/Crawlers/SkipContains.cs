using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Crawlers
{
    public class SkipContains
    {
        [Key]
        public int Id { get; set; }
        public string? Phrase { get; set; }
        public string? Reason { get; set; }
        public DateTime? Created { get; set; }
    }
}
