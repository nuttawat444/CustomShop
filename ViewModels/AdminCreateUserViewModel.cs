using System.ComponentModel.DataAnnotations;

namespace Nuttawat_7324_L002.ViewModels;

public class AdminCreateUserViewModel
{
    [Required(ErrorMessage = "กรุณากรอกชื่อ")]
    [StringLength(100, ErrorMessage = "ชื่อต้องไม่เกิน 100 ตัวอักษร")]
    [Display(Name = "ชื่อ - นามสกุล")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
    [StringLength(50, ErrorMessage = "ชื่อผู้ใช้ต้องไม่เกิน 50 ตัวอักษร")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "ชื่อผู้ใช้ใช้ได้เฉพาะตัวอักษรภาษาอังกฤษ ตัวเลข จุด ขีดล่าง และขีดกลาง")]
    [Display(Name = "ชื่อผู้ใช้ (Username)")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกอีเมล")]
    [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
    [StringLength(100, ErrorMessage = "อีเมลต้องไม่เกิน 100 ตัวอักษร")]
    [Display(Name = "อีเมล")]
    public string EmailUser { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
    [MinLength(8, ErrorMessage = "รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร")]
    [DataType(DataType.Password)]
    [Display(Name = "รหัสผ่าน")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
    [Compare(nameof(Password), ErrorMessage = "รหัสผ่านและการยืนยันรหัสผ่านไม่ตรงกัน")]
    [DataType(DataType.Password)]
    [Display(Name = "ยืนยันรหัสผ่าน")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์")]
    [Phone(ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")]
    [StringLength(20, ErrorMessage = "เบอร์โทรศัพท์ต้องไม่เกิน 20 ตัวอักษร")]
    [Display(Name = "เบอร์โทรศัพท์")]
    public string Tel { get; set; } = string.Empty;

    [Display(Name = "สิทธิ์ผู้ใช้งาน (Role)")]
    public int? RoleId { get; set; }

    public List<AdminRoleOptionViewModel> Roles { get; set; } = new();
}

public class AdminRoleOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
