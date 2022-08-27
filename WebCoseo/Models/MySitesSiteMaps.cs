using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class MySitesSiteMaps
    {
        [Key]
        public int Id { get; set; }
        public int? MySiteId { get; set; }
        public string? Status { get; set; }
        public string? SitemapUrl { get; set; }
        public Boolean? IsSitemapIndex { get; set; }
        public DateTime? LastRead { get; set; }
        public DateTime? NextRead { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
