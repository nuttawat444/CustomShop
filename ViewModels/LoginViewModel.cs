using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "กรุณากรอกอีเมล")]
    [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
    [StringLength(100, ErrorMessage = "อีเมลต้องไม่เกิน 100 ตัวอักษร")]
    public string EmailUser { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
    public string Password { get; set; } = string.Empty;
}
