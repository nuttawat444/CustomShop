namespace Nuttawat_7324_L002.Services;

public static class PartCategoryLabels
{
    public static string Label(string category) => category switch
    {
        "helmet" => "หมวกกันน็อก",
        "exhaust" => "ท่อไอเสียและอุปกรณ์",
        "parts" => "อะไหล่แต่ง",
        "chassis" => "โครงรถ",
        "powertrain" => "เครื่องยนต์",
        "suspension" => "ช่วงล่าง",
        "wheels" => "ล้อและยาง",
        "brakes" => "เบรก",
        "drivetrain" => "ระบบส่งกำลัง",
        "fuel" => "ระบบเชื้อเพลิง",
        "electrical" => "ระบบไฟ",
        "controls" => "แฮนด์และชุดควบคุม",
        "bodywork" => "ตัวถังและเบาะ",
        "accessory" => "อุปกรณ์เสริม",
        _ => category
    };
}
