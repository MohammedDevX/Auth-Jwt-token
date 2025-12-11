namespace User_service.DTOS
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string NomUser { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
