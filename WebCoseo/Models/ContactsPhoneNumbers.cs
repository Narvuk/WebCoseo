using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class ContactsPhoneNumbers
    {
        [Key]
        public int Id { get; set; }
        public int? ContactId { get; set; }
        public string? Type { get; set; }
        public string? Number { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
