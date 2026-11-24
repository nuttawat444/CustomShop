using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "กรุณากรอกอีเมลที่ลงทะเบียนไว้")]
    [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
    [Display(Name = "อีเมลที่ลงทะเบียน")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกรหัสผ่านใหม่")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "รหัสผ่านต้องมีความยาวอย่างน้อย {2} ตัวอักษร")]
    [DataType(DataType.Password)]
    [Display(Name = "รหัสผ่านใหม่")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกยืนยันรหัสผ่านใหม่")]
    [DataType(DataType.Password)]
    [Display(Name = "ยืนยันรหัสผ่านใหม่")]
    [Compare("NewPassword", ErrorMessage = "รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
