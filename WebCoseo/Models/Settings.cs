using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? SettingKey { get; set; }
        public string? SettingValue { get; set; }

    }
}
