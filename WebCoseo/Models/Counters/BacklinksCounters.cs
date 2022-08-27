using System.ComponentModel.DataAnnotations;

namespace WebCoseo.Models.Counters
{
    public class BacklinksCounters
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CounterKey { get; set; }
        public int CounterValue { get; set; } = 0;

    }
}
