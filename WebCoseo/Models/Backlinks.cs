using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Backlinks
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? BacklinkUrl { get; set; }
        public int WebsiteId { get; set; }
        public int MySiteId { get; set; }
        public int MySitePageId { get; set; }
        public string? Description { get; set; }
        public DateTime? LastChecked { get; set; }
        public DateTime? NextCheck { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
