using HgznMes.Application.Dtos.Base;
using HgznMes.Domain.ValueObjects.UserValue;

namespace HgznMes.Application.Dtos
{
    public class UserRegisterDto : CreateDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string TelephoneNumber { get; set; } = null!;
        public DateTime RegisterTime { get; set; }
    }

    public class UserReadDto : ReadDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string TelephoneNumber { get; set; } = null!;
        public RoleReadDto Role { get; set; } = null!;
        public DateTime RegisterTime { get; set; }
        public UserSettingReadDto? Settings { get; set; }
        public UserDetailReadDto? Detail { get; set; }
    }

    public class UserLoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public CaptchaAnswerDto? Captcha { get; set; }
    }

    public class UserSettingReadDto : ReadDto
    {
        public string Language { get; set; } = "Chinese";
    }

    public class UserDetailReadDto : ReadDto
    {
        public Gender Gender { get; set; } = Gender.Unknow;
        public string? AboutMe { get; set; }
    }

    public class CaptchaReadDto : ReadDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public int[] Pixel { get; set; } = null!;
        public string Image { get; set; } = null!;
    }

    public class CaptchaAnswerDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Answer { get; set; } = null!;
    }

    public class ChangePasswordDto : UpdateDto
    {
        public string Username { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public CaptchaAnswerDto? Captcha { get; set; }
    }
}