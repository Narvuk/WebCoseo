using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Websites
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

    }
}
