namespace User_service.DTOS
{
    public class RefreshTokenRequestDTO
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
