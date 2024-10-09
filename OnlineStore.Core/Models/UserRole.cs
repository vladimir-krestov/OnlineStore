using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models
{
    public class UserRole : DbModel
    {
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
