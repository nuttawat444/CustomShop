-- Run after shop-bike-demo-catalog.sql. Existing stock quantities and pricing are preserved on reruns.
-- Prices are indicative and inventory starts at zero until counted by the shop.
DROP TEMPORARY TABLE IF EXISTS seed_bike_alternatives, seed_bike_platform_slots, seed_bike_option_validation;
CREATE TEMPORARY TABLE seed_bike_alternatives (
    SlotCode VARCHAR(40) PRIMARY KEY,
    SourceSlug VARCHAR(100) NOT NULL,
    PartName VARCHAR(220) NOT NULL,
    PartDescription VARCHAR(220) NOT NULL,
    IndicativePrice DECIMAL(14,2) NOT NULL
) DEFAULT CHARSET=utf8mb4;

INSERT INTO seed_bike_alternatives VALUES
    ('frame', 'build-frame', 'ชุดเฟรมพร้อมซับเฟรมถอดได้', 'ซับเฟรมหลังถอดแยกเพื่อเข้าถึงชุดสายไฟและฐานเบาะ', 32900),
    ('engine', 'build-engine', 'ชุดเครื่องยนต์และเกียร์พร้อมคลัตช์เสริมแรง', 'ชุดส่งกำลังพร้อมจานคลัตช์และสปริงคลัตช์เสริมแรง', 78900),
    ('front-suspension', 'build-front-suspension', 'ชุดโช้คหน้าคาร์ทริดจ์ปรับหนืด', 'โช้คหน้าคาร์ทริดจ์พร้อมแผงคอสำหรับปรับแรงหน่วง', 21900),
    ('rear-suspension', 'build-rear-suspension', 'ชุดโช้คหลังปรับพรีโหลด', 'โช้คหลังพร้อมตัวปรับพรีโหลดสปริง', 14900),
    ('wheels', 'build-wheels', 'ชุดล้อซี่ลวดพร้อมยางถนน', 'ล้อซี่ลวดหน้าและหลังพร้อมยางดอกถนน', 28900),
    ('brakes', 'build-brakes', 'ชุดเบรกจานลอยและสายถัก', 'จานเบรกแบบลอยและสายเบรกถักพร้อมอุปกรณ์ยึด', 21900),
    ('final-drive', 'build-final-drive', 'ชุดโซ่ X-ring และสเตอร์', 'โซ่ซีล X-ring พร้อมสเตอร์หน้าและหลัง', 6900),
    ('fuel', 'build-fuel', 'ชุดถังน้ำมันพร้อมแผ่นกันน้ำมันกระฉอก', 'ถังพร้อมแผ่นกั้นภายใน ฝาถัง และชุดจ่ายเชื้อเพลิง', 14900),
    ('electrical', 'build-electrical', 'ชุดสายไฟพร้อมมาตรวัดดิจิทัล', 'สายไฟหลักพร้อมมาตรวัดดิจิทัลและปลั๊กชุดควบคุม', 13900),
    ('headlamp', 'product-projector', 'ชุดไฟหน้า LED วงแหวน', 'โคมไฟหน้า LED พร้อมไฟหรี่วงแหวนและขายึด', 4900),
    ('signal-light', 'build-signal-light', 'ชุดไฟสัญญาณ LED ทรงกะทัดรัด', 'ไฟท้าย ไฟเลี้ยว และไฟป้าย LED พร้อมขายึด', 5900),
    ('controls', 'build-controls', 'ชุดแฮนด์พร้อมก้านมือปรับระยะ', 'แฮนด์และชุดควบคุมพร้อมก้านเบรกและก้านคลัตช์ปรับระยะ', 9900),
    ('mirrors', 'product-bar-mirrors', 'ชุดกระจกก้านยึดประกับแฮนด์', 'กระจกคู่พร้อมก้านและประกับยึดบนแฮนด์', 2190),
    ('exhaust-kit', 'build-exhaust-kit', 'ชุดท่อไอเสียสเตนเลสพร้อมแผ่นกันร้อน', 'คอท่อและหม้อพักสเตนเลสพร้อมแผ่นกันความร้อน', 15900),
    ('seat', 'build-seat', 'ชุดเบาะโฟมสองความหนาแน่น', 'เบาะโฟมสองชั้นพร้อมฐานและชุดยึด', 6900),
    ('body-hardware', 'build-body-hardware', 'ชุดตัวถังพร้อมบังโคลนอะลูมิเนียม', 'บังโคลนอะลูมิเนียมและชุดขาตั้งพร้อมตัวยึด', 10900),
    ('luggage-rack', 'build-luggage-rack', 'แร็กท้ายพร้อมหูคล้องสายรัด', 'แร็กท้ายพร้อมหูคล้องสายรัดสัมภาระและชุดขายึด', 4290);

CREATE TEMPORARY TABLE seed_bike_platform_slots (
    BikeId INT NOT NULL,
    SlotCode VARCHAR(40) NOT NULL,
    PlatformCode VARCHAR(20) NOT NULL,
    PlatformName VARCHAR(20) NOT NULL,
    PRIMARY KEY (BikeId, SlotCode)
) DEFAULT CHARSET=utf8mb4;

CREATE TEMPORARY TABLE seed_bike_option_validation (
    IsValid BOOLEAN NOT NULL CHECK (IsValid = TRUE)
);

START TRANSACTION;

-- Expanded models inherit their platform's original frame SKU and share its inventory.
INSERT INTO seed_bike_platform_slots (BikeId, SlotCode, PlatformCode, PlatformName)
SELECT slot.BikeId, slot.SlotCode, SUBSTRING(frameStock.ItemCode, 13), frameStock.Size
FROM shop_bike_part_slots slot
JOIN shop_catalog bike ON bike.Id = slot.BikeId AND bike.Kind = 'Bike'
JOIN shop_bike_part_slots frame ON frame.BikeId = bike.Id AND frame.SlotCode = 'frame'
JOIN shop_stock frameStock ON frameStock.Id = frame.DefaultStockItemId
JOIN seed_bike_alternatives alternative ON alternative.SlotCode = slot.SlotCode
WHERE frameStock.ItemCode IN ('BUILD-FRAME-NEO400', 'BUILD-FRAME-IRON650', 'BUILD-FRAME-DESERT500',
                              'BUILD-FRAME-TRACK300', 'BUILD-FRAME-BRAT400', 'BUILD-FRAME-CHOP750');

INSERT INTO shop_catalog (Kind, Slug, Name, Category, Description, Features, Image, Price, OriginalPrice, Active)
SELECT 'Product', CONCAT('build-alt-', alternative.SlotCode), alternative.PartName, source.Category,
       CONCAT(alternative.PartDescription, ' (ราคาเบื้องต้น ภาพประกอบอ้างอิง โปรดยืนยันสเปกกับร้าน)'),
       '', source.Image, alternative.IndicativePrice, 0, TRUE
FROM seed_bike_alternatives alternative
JOIN shop_catalog source ON source.Slug = alternative.SourceSlug AND source.Kind = 'Product'
ON DUPLICATE KEY UPDATE Slug = shop_catalog.Slug;

INSERT INTO shop_stock (CatalogItemId, ItemCode, ItemName, Category, Size, QuantityOnHand, MinRequired, Unit, Location)
SELECT DISTINCT product.Id, CONCAT(UPPER(product.Slug), '-', target.PlatformCode), product.Name,
       product.Category, target.PlatformName, 0, 1, 'ชุด', 'Build parts'
FROM seed_bike_platform_slots target
JOIN shop_catalog product ON product.Slug = CONCAT('build-alt-', target.SlotCode) AND product.Kind = 'Product'
ON DUPLICATE KEY UPDATE ItemCode = shop_stock.ItemCode;

INSERT INTO shop_bike_part_options (BikeId, SlotCode, StockItemId)
SELECT target.BikeId, target.SlotCode, stock.Id
FROM seed_bike_platform_slots target
JOIN shop_catalog product ON product.Slug = CONCAT('build-alt-', target.SlotCode) AND product.Kind = 'Product'
JOIN shop_stock stock ON stock.CatalogItemId = product.Id
    AND stock.ItemCode = CONCAT(UPPER(product.Slug), '-', target.PlatformCode)
    AND stock.Size = target.PlatformName
ON DUPLICATE KEY UPDATE StockItemId = shop_bike_part_options.StockItemId;

-- Fail the batch if any active bike slot lacks two distinct active products.
INSERT INTO seed_bike_option_validation (IsValid)
SELECT NOT EXISTS (
    SELECT slot.BikeId, slot.SlotCode
    FROM shop_bike_part_slots slot
    JOIN shop_catalog bike ON bike.Id = slot.BikeId AND bike.Kind = 'Bike' AND bike.Active
    LEFT JOIN shop_bike_part_options choice ON choice.BikeId = slot.BikeId AND choice.SlotCode = slot.SlotCode
    LEFT JOIN shop_stock stock ON stock.Id = choice.StockItemId
    LEFT JOIN shop_catalog product ON product.Id = stock.CatalogItemId AND product.Kind = 'Product' AND product.Active
    GROUP BY slot.BikeId, slot.SlotCode
    HAVING COUNT(DISTINCT product.Id) < 2
);

COMMIT;
DROP TEMPORARY TABLE seed_bike_alternatives, seed_bike_platform_slots, seed_bike_option_validation;
