using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class MySitesPages
    {
        [Key]
        public int Id { get; set; }
        public int? MySiteId { get; set; }
        public string? PageName { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int? BacklinksCount { get; set; } = 0;
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
