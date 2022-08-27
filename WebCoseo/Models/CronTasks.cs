using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models
{
    public class CronTasks
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
        public string? CronKey { get; set; }
        public int? DelayTimer { get; set; }

    }
}
