using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Emails
    {
        [Key]
        public int Id { get; set; }
        public int EmailAccountId { get; set; }
        public int EmailUid { get; set; }
        public string? Folder { get; set; }
        public string? Subject { get; set; }
        public string? MessageHtml { get; set; }
        public bool? IsRead { get; set; } = false;
        public DateTime? DateReceived { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
