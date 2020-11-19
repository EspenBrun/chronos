using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronos.Models
{
    public class TimeBlock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public TimeSpan Worked { get; set; }
    }
}