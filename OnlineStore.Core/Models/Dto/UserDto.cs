namespace OnlineStore.Core.Models.Dto
{
    public class UserDto
    {
        public UserDto()
        {
            
        }

        public UserDto(User user)
        {
            Email = user.Email;
            Name = user.Name;
            PhoneNumber = user.PhoneNumber;
            Address = user.Address;
        }

        public string Email { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }
    }
}
