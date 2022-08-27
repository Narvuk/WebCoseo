using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class EmailAccounts
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? ImapServer { get; set; }
        public string? ImapPort { get; set; }
        public string? ImapUsername { get; set; }
        public string? ImapPassword { get; set; }
        public string? SmtpServer { get; set; }
        public string? SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public DateTime? Created { get; set; }


    }
}
