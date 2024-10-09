using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models
{
    public abstract class DbModel
    {
        [Key]
        public int Id { get; set; }
    }
}
