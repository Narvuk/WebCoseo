using System;
using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class ContactsSocialMedia
    {
        [Key]
        public int Id { get; set; }
        public int? ContactId { get; set; }
        public int? SocialMediaId { get; set; }
        public string? Url { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
