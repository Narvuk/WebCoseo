using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class SystemConfig
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ConfigKey { get; set; }
        public string? ConfigValue { get; set; }


    }
}
