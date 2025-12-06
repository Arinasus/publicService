using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class UserDto
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; } = string.Empty;
    }
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
