using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class ContactsWebsites
    {
        [Key]
        public int Id { get; set; }
        public int? ContactId { get; set; }
        public int? WebsiteId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
