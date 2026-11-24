-- Demo catalog for the six existing Custom Bikes and six new ready-to-buy motorcycles.
-- Repeat runs preserve prices, stock already entered and part configuration.
START TRANSACTION;
INSERT INTO shop_catalog (Kind, Slug, Name, Category, Description, Features, Image, Price, OriginalPrice, Active)
VALUES
    ('Package','ready-solstice-racer','Solstice Racer 450','Cafe Racer','รถคาเฟ่เรเซอร์สีเงินฟ้า ทรงหมอบ เบาะเดี่ยวสีแทน','เครื่องยนต์ 450cc · เบาะทรงสปอร์ต · แฮนด์จับโช้ค','ready-solstice-racer.jpg',189900,209900,TRUE),
    ('Package','ready-copperline-bobber','Copperline Bobber 700','Bobber','บ็อบเบอร์เบาะเดี่ยว ถังสีทองแดง ล้อแก้มสูง','เครื่องยนต์ 700cc · เบาะเดี่ยว · ล้อซี่ลวด','ready-copperline-bobber.jpg',249900,279900,TRUE),
    ('Package','ready-pinecrest-scrambler','Pinecrest Scrambler 520','Scrambler','สแครมเบลอร์ถังเขียวเข้ม ท่อสูงและยางกึ่งวิบาก','เครื่องยนต์ 520cc · ยางกึ่งวิบาก · ท่อยกสูง','ready-pinecrest-scrambler.jpg',209900,229900,TRUE),
    ('Package','ready-signal-tracker','Signal Street Tracker 350','Street Tracker','สตรีตแทร็กเกอร์ถังขาวส้ม ทรงกะทัดรัด','เครื่องยนต์ 350cc · แฮนด์กว้าง · ท้ายแทร็กเกอร์','ready-signal-tracker.jpg',159900,179900,TRUE),
    ('Package','ready-midnight-brat','Midnight Brat 420','Brat Style','รถทรงแบรตถังสีน้ำเงินเข้ม เบาะแบนเย็บลอน','เครื่องยนต์ 420cc · เบาะยาวเย็บลอน · ล้อซี่ลวด','ready-midnight-brat.jpg',179900,199900,TRUE),
    ('Package','ready-meridian-chopper','Meridian Highway Chopper 800','Chopper','ชอปเปอร์สีครีมคาดแดง โช้คหน้าทรงยาว','เครื่องยนต์ 800cc · โช้คหน้ายาว · พนักพิงสูง','ready-meridian-chopper.jpg',299900,329900,TRUE),
    ('Product','build-frame','ชุดโครงรถและสวิงอาร์ม','chassis','โครงหลัก ซับเฟรม สวิงอาร์ม และชุดลูกปืนคอ แยกตามรุ่น','','build-frame.jpg',24900,0,TRUE),
    ('Product','build-engine','ชุดเครื่องยนต์และเกียร์','powertrain','เครื่องยนต์ เกียร์ คลัตช์ ชุดไอดี และระบบระบายความร้อนตามรุ่น','','build-engine.jpg',64900,0,TRUE),
    ('Product','build-front-suspension','ชุดโช้คหน้าและแผงคอ','suspension','โช้คหน้าคู่พร้อมแผงคอและแกนยึดตรงรุ่น','','build-front-suspension.jpg',13900,0,TRUE),
    ('Product','build-rear-suspension','ชุดโช้คหลัง','suspension','ชุดกันสะเทือนหลังและบูชสำหรับจุดยึดของรุ่น','','build-rear-suspension.jpg',9900,0,TRUE),
    ('Product','build-wheels','ชุดล้อหน้า–หลังและยาง','wheels','ล้อและยางหน้า–หลังพร้อมแกน ลูกปืน วาล์ว และสเปเซอร์','','build-wheels.jpg',22400,0,TRUE),
    ('Product','build-brakes','ชุดเบรกหน้า–หลัง','brakes','จานเบรก คาลิเปอร์ ปั๊ม สายเบรก และผ้าเบรกสำหรับชุดหน้า–หลัง','','build-brakes.jpg',15300,0,TRUE),
    ('Product','build-final-drive','ชุดโซ่และสเตอร์','drivetrain','โซ่ สเตอร์หน้า–หลัง และฝาครอบตรงรุ่น','','build-final-drive.jpg',5300,0,TRUE),
    ('Product','build-fuel','ชุดถังน้ำมันและจ่ายเชื้อเพลิง','fuel','ถัง ฝาถัง ปั๊มหรือก๊อก กรอง และสายเชื้อเพลิงตามรุ่น','','build-fuel.jpg',11400,0,TRUE),
    ('Product','build-electrical','ชุดสายไฟ แบตเตอรี่ และเครื่องมือวัด','electrical','สายไฟหลัก แบตเตอรี่ เรกูเลเตอร์ กล่องควบคุม และมาตรวัดตรงรุ่น','','build-electrical.jpg',10400,0,TRUE),
    ('Product','build-signal-light','ชุดไฟท้าย ไฟเลี้ยว และไฟป้าย','electrical','ไฟท้าย ไฟเลี้ยวหน้า–หลัง ไฟป้าย และขายึด','','build-signal-light.jpg',4700,0,TRUE),
    ('Product','build-controls','ชุดแฮนด์และชุดควบคุม','controls','แฮนด์ ปลอกแฮนด์ ก้านมือ สายคันเร่ง สวิตช์ และพักเท้าตรงรุ่น','','build-controls.jpg',7900,0,TRUE),
    ('Product','build-exhaust-kit','ชุดท่อไอเสียครบระบบ','exhaust','คอท่อ ปลายท่อ ขายึด และแผ่นกันร้อนตรงรุ่น','','build-exhaust-kit.jpg',11900,0,TRUE),
    ('Product','build-seat','ชุดเบาะและฐานยึด','bodywork','เบาะพร้อมฐานและขายึดตรงตำแหน่งเดิมของแต่ละรุ่น','','build-seat.jpg',4900,0,TRUE),
    ('Product','build-body-hardware','ชุดตัวถัง ขาตั้ง และตัวยึด','bodywork','บังโคลน ฝาครอบ ขาตั้ง ชุดน็อตยึด และวัสดุสิ้นเปลืองตามรุ่น','','build-body-hardware.jpg',8600,0,TRUE),
    ('Product','build-luggage-rack','แร็กท้ายคัสตอม','accessory','อุปกรณ์เสริมแร็กท้ายพร้อมชุดยึดตรงรุ่น','','build-luggage-rack.jpg',3100,0,TRUE)
ON DUPLICATE KEY UPDATE Slug = shop_catalog.Slug;

INSERT INTO shop_stock (CatalogItemId, ItemCode, ItemName, Category, Size, QuantityOnHand, MinRequired, Unit, Location)
SELECT item.Id, UPPER(item.Slug), item.Name, item.Category, '',
       CASE item.Slug WHEN 'ready-copperline-bobber' THEN 3 WHEN 'ready-meridian-chopper' THEN 1 ELSE 2 END,
       1, 'คัน', 'Ready Bike'
FROM shop_catalog item
WHERE item.Kind = 'Package' AND item.Slug IN (
    'ready-solstice-racer','ready-copperline-bobber','ready-pinecrest-scrambler',
    'ready-signal-tracker','ready-midnight-brat','ready-meridian-chopper'
)
ON DUPLICATE KEY UPDATE ItemCode = shop_stock.ItemCode;

-- Each platform gets its own stock identity and quantities.
INSERT INTO shop_stock (CatalogItemId, ItemCode, ItemName, Category, Size, QuantityOnHand, MinRequired, Unit, Location)
SELECT product.Id, CONCAT(UPPER(product.Slug), '-', platform.SkuSuffix), product.Name, product.Category,
       platform.Variant, 8, 1, 'ชุด', 'Build parts'
FROM shop_catalog product
CROSS JOIN (
    SELECT 'NEO400' AS SkuSuffix, 'Neo 400' AS Variant
    UNION ALL SELECT 'IRON650','Iron 650'
    UNION ALL SELECT 'DESERT500','Desert 500'
    UNION ALL SELECT 'TRACK300','Tracker 300'
    UNION ALL SELECT 'BRAT400','Brat 400'
    UNION ALL SELECT 'CHOP750','Chopper 750'
) platform
WHERE product.Kind = 'Product' AND product.Slug IN (
    'build-frame','build-engine','build-front-suspension','build-rear-suspension','build-wheels','build-brakes',
    'build-final-drive','build-fuel','build-electrical','build-signal-light','build-controls','build-exhaust-kit',
    'build-seat','build-body-hardware','build-luggage-rack'
)
ON DUPLICATE KEY UPDATE ItemCode = shop_stock.ItemCode;

-- Old service campaigns stay in the admin catalog and linked history.
UPDATE shop_catalog SET Active = FALSE
WHERE Kind = 'Package' AND Slug IN (
    'package-cafe','package-bobber','package-scrambler','package-paintwork',
    'package-tuning','package-voucher','season-build'
);

INSERT INTO shop_bike_part_slots (BikeId, SlotCode, SlotName, SortOrder, MinQuantity, MaxQuantity, DefaultStockItemId)
SELECT bike.Id, slot.Code, slot.Label, slot.Position, slot.Minimum, 1,
       CASE WHEN slot.Minimum = 1 THEN sku.Id ELSE NULL END
FROM shop_catalog bike
JOIN (
    SELECT 'bike-cafe' AS BikeSlug, 'NEO400' AS SkuSuffix
    UNION ALL SELECT 'bike-bobber','IRON650'
    UNION ALL SELECT 'bike-scrambler','DESERT500'
    UNION ALL SELECT 'bike-tracker','TRACK300'
    UNION ALL SELECT 'bike-brat','BRAT400'
    UNION ALL SELECT 'bike-chopper','CHOP750'
) platform ON platform.BikeSlug = bike.Slug
CROSS JOIN (
    SELECT 'frame' AS Code, 'โครงรถและสวิงอาร์ม' AS Label, 10 AS Position, 1 AS Minimum, 'BUILD-FRAME' AS SkuPrefix
    UNION ALL SELECT 'engine','เครื่องยนต์และเกียร์',20,1,'BUILD-ENGINE'
    UNION ALL SELECT 'front-suspension','ช่วงล่างหน้า',30,1,'BUILD-FRONT-SUSPENSION'
    UNION ALL SELECT 'rear-suspension','ช่วงล่างหลัง',40,1,'BUILD-REAR-SUSPENSION'
    UNION ALL SELECT 'wheels','ล้อและยางหน้า–หลัง',50,1,'BUILD-WHEELS'
    UNION ALL SELECT 'brakes','เบรกหน้า–หลัง',60,1,'BUILD-BRAKES'
    UNION ALL SELECT 'final-drive','โซ่และสเตอร์',70,1,'BUILD-FINAL-DRIVE'
    UNION ALL SELECT 'fuel','ระบบเชื้อเพลิง',80,1,'BUILD-FUEL'
    UNION ALL SELECT 'electrical','ระบบไฟและมาตรวัด',90,1,'BUILD-ELECTRICAL'
    UNION ALL SELECT 'headlamp','ไฟหน้า',100,1,'PRODUCT-PROJECTOR'
    UNION ALL SELECT 'signal-light','ไฟท้ายและไฟสัญญาณ',110,1,'BUILD-SIGNAL-LIGHT'
    UNION ALL SELECT 'controls','แฮนด์และชุดควบคุม',120,1,'BUILD-CONTROLS'
    UNION ALL SELECT 'mirrors','กระจกมองหลัง',130,1,'PRODUCT-BAR-MIRRORS'
    UNION ALL SELECT 'exhaust-kit','ระบบไอเสีย',140,1,'BUILD-EXHAUST-KIT'
    UNION ALL SELECT 'seat','เบาะและฐานยึด',150,1,'BUILD-SEAT'
    UNION ALL SELECT 'body-hardware','ตัวถัง ขาตั้ง และตัวยึด',160,1,'BUILD-BODY-HARDWARE'
    UNION ALL SELECT 'luggage-rack','แร็กท้ายเสริม',170,0,'BUILD-LUGGAGE-RACK'
) slot
JOIN shop_stock sku ON sku.ItemCode = CASE
    WHEN slot.SkuPrefix LIKE 'BUILD-%' THEN CONCAT(slot.SkuPrefix, '-', platform.SkuSuffix)
    ELSE slot.SkuPrefix
END
WHERE bike.Kind = 'Bike'
ON DUPLICATE KEY UPDATE SlotName = shop_bike_part_slots.SlotName;

INSERT INTO shop_bike_part_options (BikeId, SlotCode, StockItemId)
SELECT slots.BikeId, slots.SlotCode, slots.DefaultStockItemId
FROM shop_bike_part_slots slots
JOIN shop_catalog bike ON bike.Id = slots.BikeId
WHERE slots.DefaultStockItemId IS NOT NULL AND bike.Slug IN
    ('bike-cafe','bike-bobber','bike-scrambler','bike-tracker','bike-brat','bike-chopper')
ON DUPLICATE KEY UPDATE StockItemId = shop_bike_part_options.StockItemId;

INSERT INTO shop_bike_part_options (BikeId, SlotCode, StockItemId)
SELECT slot.BikeId, slot.SlotCode, sku.Id
FROM shop_bike_part_slots slot
JOIN shop_catalog bike ON bike.Id = slot.BikeId
JOIN shop_stock sku ON sku.ItemCode = CASE
    WHEN slot.SlotCode = 'headlamp' THEN 'PRODUCT-BRASS'
    WHEN slot.SlotCode = 'seat' AND bike.Slug = 'bike-cafe' THEN 'PRODUCT-HUMP-SEAT'
    WHEN slot.SlotCode = 'seat' AND bike.Slug = 'bike-brat' THEN 'PRODUCT-FLAT-SEAT'
    WHEN slot.SlotCode = 'luggage-rack' THEN CONCAT('BUILD-LUGGAGE-RACK-', CASE bike.Slug
        WHEN 'bike-cafe' THEN 'NEO400' WHEN 'bike-bobber' THEN 'IRON650'
        WHEN 'bike-scrambler' THEN 'DESERT500' WHEN 'bike-tracker' THEN 'TRACK300'
        WHEN 'bike-brat' THEN 'BRAT400' WHEN 'bike-chopper' THEN 'CHOP750'
    END)
END
WHERE bike.Slug IN ('bike-cafe','bike-bobber','bike-scrambler','bike-tracker','bike-brat','bike-chopper')
ON DUPLICATE KEY UPDATE StockItemId = shop_bike_part_options.StockItemId;

COMMIT;
