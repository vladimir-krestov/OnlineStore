namespace OnlineStore.Core.Models
{
    public class UserRoleMapping
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public UserRole Role { get; set; }
    }
}
