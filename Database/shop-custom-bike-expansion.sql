-- Twelve demo builds for the six existing Custom Bike platforms.
SET NAMES utf8mb4;

CREATE TEMPORARY TABLE seed_custom_bikes (
    Slug VARCHAR(100) PRIMARY KEY,
    BaseSlug VARCHAR(100) NOT NULL,
    Name VARCHAR(220) NOT NULL UNIQUE,
    Description TEXT NOT NULL,
    Features TEXT NOT NULL,
    Image VARCHAR(120) NOT NULL,
    Price DECIMAL(14,2) NOT NULL CHECK (Price > 0),
    BaseId INT NULL,
    NeedsInsert BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT INTO seed_custom_bikes (Slug, BaseSlug, Name, Description, Features, Image, Price)
VALUES
    ('bike-cafe-apex-400', 'bike-cafe', 'Apex Café Racer 400',
     'คาเฟ่เรเซอร์สำหรับคนชอบทรงหมอบ แนวถังและเบาะต่อเนื่องกันแบบรถแข่งคลาสสิก',
     'เครื่องยนต์ · 400cc Parallel Twin\nแนวแต่ง · แฮนด์จับโช้คและเบาะเดี่ยวทรงสปอร์ต\nระยะเวลาสร้างโดยประมาณ · 45 วันทำการ',
     'cafe.jpg', 199000),
    ('bike-cafe-silverline-400', 'bike-cafe', 'Silverline Café 400',
     'คาเฟ่เรเซอร์เส้นสายเรียบ เน้นผิวโลหะและงานเก็บรายละเอียด เหมาะกับคนที่ชอบแต่งแบบคลาสสิก',
     'เครื่องยนต์ · 400cc Parallel Twin\nแนวแต่ง · งานผิวโลหะและเบาะทรงท้ายสั้น\nระยะเวลาสร้างโดยประมาณ · 45 วันทำการ',
     'ready-solstice-racer.jpg', 209000),
    ('bike-bobber-nightfall-650', 'bike-bobber', 'Nightfall Bobber 650',
     'บ็อบเบอร์ทรงต่ำเน้นงานผิวด้าน เบาะเดี่ยวและบังโคลนสั้นช่วยขับเส้นสายของตัวรถ',
     'เครื่องยนต์ · 650cc V-Twin Air-Cooled\nแนวแต่ง · เบาะเดี่ยวทรงต่ำและถังทรงหยดน้ำ\nระยะเวลาสร้างโดยประมาณ · 60 วันทำการ',
     'bobber.jpg', 249000),
    ('bike-bobber-foundry-650', 'bike-bobber', 'Foundry Bobber 650',
     'บ็อบเบอร์กลิ่นอายงานโลหะ เน้นพื้นผิวถังที่ตัดกับชิ้นส่วนสีเข้มและโครงท้ายแบบเรียบ',
     'เครื่องยนต์ · 650cc V-Twin Air-Cooled\nแนวแต่ง · ถังแต่งผิวโลหะและแฮนด์ทรงตรง\nระยะเวลาสร้างโดยประมาณ · 60 วันทำการ',
     'ready-copperline-bobber.jpg', 259000),
    ('bike-scrambler-ridgeline-500', 'bike-scrambler', 'Ridgeline Scrambler 500',
     'สแครมเบลอร์สำหรับทริปที่สลับระหว่างถนนกับทางลูกรัง ใช้แนวแต่งแฮนด์กว้างและเบาะยาว',
     'เครื่องยนต์ · 500cc High-Torque Single\nแนวแต่ง · ยางกึ่งวิบากและการ์ดใต้เครื่อง\nระยะเวลาสร้างโดยประมาณ · 40 วันทำการ',
     'scrambler.jpg', 205000),
    ('bike-scrambler-fieldstone-500', 'bike-scrambler', 'Fieldstone Scrambler 500',
     'สแครมเบลอร์แนวทัวริ่งระยะสั้น เว้นพื้นที่ท้ายรถสำหรับเลือกเพิ่มแร็กสัมภาระตอนจัดสเปก',
     'เครื่องยนต์ · 500cc High-Torque Single\nแนวแต่ง · เบาะยาวและตัวเลือกแร็กท้าย\nระยะเวลาสร้างโดยประมาณ · 45 วันทำการ',
     'ready-pinecrest-scrambler.jpg', 215000),
    ('bike-tracker-circuit-300', 'bike-tracker', 'Circuit Flat Tracker 300',
     'แฟลตแทร็กเกอร์ที่หยิบเส้นสายรถแข่งมาปรับใช้กับรถแต่งประจำวัน เน้นถังทรงแคบและท้ายสั้น',
     'เครื่องยนต์ · 300cc DOHC ระบายความร้อนด้วยน้ำ\nแนวแต่ง · ป้ายเบอร์และล้อซี่ลวด\nระยะเวลาสร้างโดยประมาณ · 30 วันทำการ',
     'tracker.jpg', 149000),
    ('bike-tracker-grid-300', 'bike-tracker', 'Grid Street Tracker 300',
     'สตรีตแทร็กเกอร์แนวเมือง เน้นช่วงแฮนด์กว้างและงานเก็บสายไฟรอบชุดหน้ารถ',
     'เครื่องยนต์ · 300cc DOHC ระบายความร้อนด้วยน้ำ\nแนวแต่ง · ท้ายกะทัดรัดและไฟหน้าทรงกลม\nระยะเวลาสร้างโดยประมาณ · 35 วันทำการ',
     'ready-signal-tracker.jpg', 155000),
    ('bike-brat-sumi-400', 'bike-brat', 'Sumi Brat 400',
     'รถแต่งสไตล์ญี่ปุ่นที่เน้นแนวถังกับเบาะอยู่ระดับเดียวกัน รูปทรงเรียบและมีรายละเอียดน้อยชิ้น',
     'เครื่องยนต์ · 400cc Single Thumper\nแนวแต่ง · เบาะแบนเย็บลายและไมล์เดี่ยว\nระยะเวลาสร้างโดยประมาณ · 35 วันทำการ',
     'brat.jpg', 175000),
    ('bike-brat-after-hours-400', 'bike-brat', 'After Hours Brat 400',
     'รถแต่งสำหรับคนที่ชอบงานโทนเข้มและท่านั่งผ่อนคลาย เน้นเบาะแนวยาวกับชุดไฟทรงคลาสสิก',
     'เครื่องยนต์ · 400cc Single Thumper\nแนวแต่ง · เบาะยาวและไฟหน้าทรงกลม\nระยะเวลาสร้างโดยประมาณ · 40 วันทำการ',
     'ready-midnight-brat.jpg', 182000),
    ('bike-chopper-longmile-750', 'bike-chopper', 'Longmile Chopper 750',
     'ช็อปเปอร์ทรงยาวที่เน้นแนวโช้คหน้าและล้อซี่ลวด เลือกเก็บรายละเอียดงานโลหะแบบอเมริกันคลาสสิก',
     'เครื่องยนต์ · 750cc Long-Stroke\nแนวแต่ง · ล้อหน้า 21 นิ้วและบาร์หลังทรงสูง\nระยะเวลาสร้างโดยประมาณ · 90 วันทำการ',
     'chopper.jpg', 329000),
    ('bike-chopper-horizon-750', 'bike-chopper', 'Horizon Chopper 750',
     'ช็อปเปอร์ที่เน้นถังเพนต์และสัดส่วนล้อหน้าเด่น เหมาะกับคนที่ต้องการกำหนดรายละเอียดรถตั้งแต่ต้น',
     'เครื่องยนต์ · 750cc Long-Stroke\nแนวแต่ง · ถังเพนต์และบาร์หลังขึ้นรูป\nระยะเวลาสร้างโดยประมาณ · 95 วันทำการ',
     'ready-meridian-chopper.jpg', 345000);

CREATE TEMPORARY TABLE seed_custom_bike_validation (
    HasValidPlatforms BOOLEAN NOT NULL DEFAULT TRUE CHECK (HasValidPlatforms = TRUE),
    HasValidSlugs BOOLEAN NOT NULL DEFAULT TRUE CHECK (HasValidSlugs = TRUE)
) ENGINE=InnoDB;

START TRANSACTION;

UPDATE seed_custom_bikes seed
LEFT JOIN shop_catalog platform ON platform.Slug = seed.BaseSlug
    AND platform.Kind = 'Bike' AND platform.Active
LEFT JOIN shop_catalog existing ON existing.Slug = seed.Slug
SET seed.BaseId = platform.Id, seed.NeedsInsert = existing.Id IS NULL;

-- A failed check aborts the batch before publication. Run with mysql without --force.
INSERT INTO seed_custom_bike_validation (HasValidPlatforms)
SELECT NOT EXISTS (
    SELECT 1
    FROM seed_custom_bikes seed
    LEFT JOIN shop_bike_part_slots slot ON slot.BikeId = seed.BaseId
    WHERE seed.NeedsInsert
    GROUP BY seed.Slug
    HAVING COUNT(slot.SlotCode) <> 17
        OR COUNT(CASE WHEN slot.MinQuantity = 1 THEN 1 END) <> 16
        OR COUNT(CASE WHEN slot.MinQuantity = 0 THEN 1 END) <> 1
        OR COUNT(CASE WHEN slot.MaxQuantity = 1 THEN 1 END) <> 17
        OR COUNT(CASE WHEN NOT EXISTS (
            SELECT 1
            FROM shop_bike_part_options choice
            JOIN shop_stock stock ON stock.Id = choice.StockItemId
            JOIN shop_catalog product ON product.Id = stock.CatalogItemId
            WHERE choice.BikeId = slot.BikeId AND choice.SlotCode = slot.SlotCode
                AND product.Kind = 'Product' AND product.Active
                AND (slot.MinQuantity = 0 OR choice.StockItemId = slot.DefaultStockItemId)
        ) THEN 1 END) > 0
);

INSERT INTO seed_custom_bike_validation (HasValidSlugs)
SELECT NOT EXISTS (
    SELECT 1
    FROM seed_custom_bikes seed
    JOIN shop_catalog existing ON existing.Slug = seed.Slug
    WHERE existing.Kind <> 'Bike'
);

INSERT INTO shop_catalog (Kind, Slug, Name, Category, Description, Features, Image, Price, OriginalPrice, Active)
SELECT 'Bike', seed.Slug, seed.Name, platform.Category,
    CONCAT(seed.Description, ' (ภาพประกอบแสดงแนวแต่ง)'), seed.Features, seed.Image, seed.Price, 0, TRUE
FROM seed_custom_bikes seed
JOIN shop_catalog platform ON platform.Id = seed.BaseId
WHERE seed.NeedsInsert
ORDER BY platform.Id, seed.Price;

-- Keep the original StockItemId so variants share the same inventory.
INSERT INTO shop_bike_part_slots (BikeId, SlotCode, SlotName, SortOrder, MinQuantity, MaxQuantity, DefaultStockItemId)
SELECT bike.Id, slot.SlotCode, slot.SlotName, slot.SortOrder, slot.MinQuantity, slot.MaxQuantity,
    slot.DefaultStockItemId
FROM seed_custom_bikes seed
JOIN shop_catalog bike ON bike.Slug = seed.Slug
JOIN shop_bike_part_slots slot ON slot.BikeId = seed.BaseId
WHERE seed.NeedsInsert;

INSERT INTO shop_bike_part_options (BikeId, SlotCode, StockItemId)
SELECT bike.Id, choice.SlotCode, choice.StockItemId
FROM seed_custom_bikes seed
JOIN shop_catalog bike ON bike.Slug = seed.Slug
JOIN shop_bike_part_options choice ON choice.BikeId = seed.BaseId
WHERE seed.NeedsInsert;

COMMIT;

DROP TEMPORARY TABLE seed_custom_bike_validation, seed_custom_bikes;
