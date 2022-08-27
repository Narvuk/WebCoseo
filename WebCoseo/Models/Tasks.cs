using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Tasks
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Title { get; set; }
        public long? Description { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
