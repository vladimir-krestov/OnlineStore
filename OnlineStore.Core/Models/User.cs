using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models
{
    public class User : DbModel
    {
        // Personal data
        [MaxLength(255, ErrorMessage = "Email can't contain more that 255 symbols.")]
        public string Email { get; set; }

        [MaxLength(100, ErrorMessage = "Name can't be more 100 symbols.")]
        public string Name { get; set; }

        [MaxLength(15, ErrorMessage = "Phone number can't be more 15 symbols.")]
        public string PhoneNumber { get; set; }

        [MaxLength(200, ErrorMessage = "Phone number can't be more 200 symbols.")]
        public string Address { get; set; }


        // System data
        public List<UserRoleMapping> Roles { get; set; } = new();

        [MaxLength(44, ErrorMessage = "Password hash must be 44 symbols for the current configuration.")]
        public string PasswordHash { get; set; }

        [MaxLength(24, ErrorMessage = "Salt must be 24 symbols length.")]
        public string Salt { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
