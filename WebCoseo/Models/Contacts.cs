using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Contacts
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactSource { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

    }
}
