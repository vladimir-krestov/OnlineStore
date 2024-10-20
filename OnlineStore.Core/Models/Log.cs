using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models
{
    public class Log : DbModel
    {
        public DateTime TimeStamp { get; set; }

        [MaxLength(20)]
        public string Level { get; set; }

        [MaxLength(200)]
        public string Message { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
