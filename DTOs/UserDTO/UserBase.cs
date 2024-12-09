namespace Api.DTOs.UserDTO
{
    public abstract class UserBase
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;

    }
}
