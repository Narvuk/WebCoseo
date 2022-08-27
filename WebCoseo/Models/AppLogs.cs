using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class AppLogs
    {
        [Key]
        public int Id { get; set; }
        public string? Section { get; set; }
        public string? Type { get; set; }
        public DateTime? Created { get; set; }
        public string? Message { get; set; }

    }
}
