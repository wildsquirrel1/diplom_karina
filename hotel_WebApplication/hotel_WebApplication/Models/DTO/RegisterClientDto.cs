namespace hotel_WebApplication.Models.DTO
{
    public class RegisterClientDto
    {
        public string Name { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string SeriaPass { get; set; } = null!;
        public string NumberPass { get; set; } = null!;
        public string Birth { get; set; } = null!; // string, а не DateOnly
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
