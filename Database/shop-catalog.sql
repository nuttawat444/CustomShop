START TRANSACTION;
-- Seed the existing catalog once. Stock starts at zero until counted by warehouse staff.

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-moto3', 'Bell Moto-3 Retro White', 'helmet', 'หมวกกันน็อกวิบากเรโทรในตำนาน เปลือกไฟเบอร์กลาสแท้ น้ำหนักเบา ระบายอากาศเยี่ยม', 'มาตรฐาน ECE 22.06 / DOT
นวมถอดซักได้ มีระบบ Antimicrobial
สายรัดคางแบบ Double D-Ring', 'moto3.jpg', '11500', '12900', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-moto3');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-MOTO3-S',Name,Category,'S',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-moto3' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-MOTO3-S');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-MOTO3-M',Name,Category,'M',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-moto3' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-MOTO3-M');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-MOTO3-L',Name,Category,'L',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-moto3' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-MOTO3-L');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-MOTO3-XL',Name,Category,'XL',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-moto3' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-MOTO3-XL');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-gringo', 'Biltwell Gringo S Matte Black', 'helmet', 'หมวกเต็มใบสไตล์คัสตอม ผิวสัมผัสดำด้านดุดัน ชิลด์หน้ากันรอยพร้อมตัวล็อกแบบ Snap', 'ชิลด์ Polycarbonate กันรังสี UV400
นวมบุกำมะหยี่ระบายอากาศนุ่มสบาย
ผ่านมาตรฐานความปลอดภัย DOT สหรัฐฯ', 'gringo.jpg', '9200', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-gringo');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GRINGO-S',Name,Category,'S',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-gringo' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GRINGO-S');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GRINGO-M',Name,Category,'M',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-gringo' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GRINGO-M');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GRINGO-L',Name,Category,'L',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-gringo' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GRINGO-L');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GRINGO-XL',Name,Category,'XL',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-gringo' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GRINGO-XL');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-torc', 'Torc T-50 Open Face Classic', 'helmet', 'หมวกเปิดหน้า 3/4 น้ำหนักเบาเพียง 1,100 กรัม เหมาะสำหรับขี่ชิวในเมืองและทริปสั้น', 'โครงสร้าง ABS Shell เหนียวทนทาน
กระดุม 3 เม็ดสำหรับติดแก๊ปหรือชิลด์
สายรัดคางบุนวมกระชับ ปลดไว', 'torc.jpg', '4850', '5400', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-torc');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TORC-S',Name,Category,'S',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-torc' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TORC-S');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TORC-M',Name,Category,'M',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-torc' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TORC-M');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TORC-L',Name,Category,'L',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-torc' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TORC-L');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TORC-XL',Name,Category,'XL',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-torc' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TORC-XL');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-glamster', 'Shoei Glamster 06 Basalt Grey', 'helmet', 'หมวกกันน็อกระดับท็อปจากญี่ปุ่น ทรงกะทัดรัด แข็งแกร่งด้วยไฟเบอร์ AIM+ Matrix', 'แถมฟรีแผ่น Pinlock EVO กันฝ้า
ช่องรับลมหน้า-หลังออกแบบตามหลักอากาศพลศาสตร์
ระบบ E.Q.R.S. ถอดนวมฉุกเฉินได้ปลอดภัย', 'glamster.jpg', '19500', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-glamster');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GLAMSTER-S',Name,Category,'S',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-glamster' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GLAMSTER-S');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GLAMSTER-M',Name,Category,'M',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-glamster' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GLAMSTER-M');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GLAMSTER-L',Name,Category,'L',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-glamster' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GLAMSTER-L');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-GLAMSTER-XL',Name,Category,'XL',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-glamster' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-GLAMSTER-XL');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-exhaust', 'ท่อ Megaphone Stainless 304', 'exhaust', 'ปลายท่อทรงเมกาโฟนสเตนเลสเกรด 304 เสียงทุ้มแน่น เพิ่มอัตราเร่งและรอบต้น-กลาง', 'สเตนเลสสตีลเกรด 304 ปลอดสนิม
ใยแก้วซับเสียงทนความร้อนสูง 900°C
มาพร้อมขายึด Universal ปรับใส่ได้ทุกรุ่น', 'exhaust.jpg', '4200', '4900', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-exhaust');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-EXHAUST',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-exhaust' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-EXHAUST');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-projector', 'ไฟหน้า LED โปรเจกเตอร์ 7 นิ้ว', 'exhaust', 'ไฟหน้าโปรเจกเตอร์ทรงกลมคลาสสิก พร้อมไฟ DRL วงแหวนขาวนวล ลำแสงคมชัดไม่แยงตา', 'ชิป Cree LED สว่างกว่าหลอดเดิม 3 เท่า
ตัวโคมอลูมิเนียมหล่อขึ้นรูป ระบายความร้อนดีเยี่ยม
กันน้ำและฝุ่นมาตรฐาน IP68', 'projector.jpg', '2890', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-projector');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-PROJECTOR',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-projector' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-PROJECTOR');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-bar-mirrors', 'กระจกปลายแฮนด์ CNC ทรงกลม', 'parts', 'กระจกปลายแฮนด์อลูมิเนียม CNC ชุบอโนไดซ์สีดำ เลนส์ตัดแสงสีฟ้า ปรับหมุนได้ 360°', 'เลนส์นูนมุมมองกว้าง ลดจุดบอด
รองรับแฮนด์ขนาด 22mm และ 28mm
ไม่สั่นไหวที่ความเร็วสูง', 'bar-mirrors.jpg', '1450', '1800', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-bar-mirrors');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-BAR-MIRRORS',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-bar-mirrors' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-BAR-MIRRORS');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-clipons', 'แฮนด์จับโช้ค Clip-On อลูมิเนียม', 'parts', 'แฮนด์จับโช้คทรงคาเฟ่เรเซอร์ ปรับมุมก้ม-เงยได้อิสระ มอบท่านั่งสปอร์ตกระชับมือ', 'อลูมิเนียมเกรดอากาศยาน T6-6061
สำหรับแกนโช้ค 41mm (มีปลอกแปลง 37-39mm)
ชุบสีฮาร์ดอโนไดซ์ ทนรอยขีดข่วน', 'clipons.jpg', '2100', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-clipons');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-CLIPONS',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-clipons' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-CLIPONS');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-hump-seat', 'เบาะตูดมดหนังแท้ สีน้ำตาลวินเทจ', 'parts', 'เบาะคัสตอมตูดมดทรงคาเฟ่เรเซอร์ หุ้มหนังวัวแท้กันน้ำ เย็บลายขวางด้วยด้ายเหนียวพิเศษ', 'ฟองน้ำความหนาแน่นสูง ไม่ยวบตัว
ฐานรองเหล็กขึ้นรูป แข็งแรงทนทาน
แถมขายึดปรับระดับได้ 3 ตำแหน่ง', 'hump-seat.jpg', '3600', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-hump-seat');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-HUMP-SEAT',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-hump-seat' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-HUMP-SEAT');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product', 'product-tire', 'ยางฟันเลื่อย Classic 4.00-18', 'exhaust', 'ยางมอเตอร์ไซค์คลาสสิกลายฟันเลื่อยยอดนิยม แก้มยางทรงอ้วน เสริมลุคบ็อบเบอร์และคาเฟ่', 'คอมปาวด์ยางเกาะถนนแห้งและเปียกได้ดี
ร่องยางลึก รีดน้ำสไตล์เรโทร
โครงสร้าง 4 ชั้น รองรับน้ำหนักได้มาก', 'tire.jpg', '3250', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-tire');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TIRE',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-tire' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TIRE');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-cafe', 'Cafe Racer Neo 400', 'cafe', 'มอเตอร์ไซค์ทรงสปอร์ตคลาสสิก แฮนด์จับโช้ค Clip-on สวิงอาร์มขึ้นรูปพิเศษ เบาะตูดมดหนังแท้สีแทน ให้ฟีลลิ่งซิ่งสไตล์อังกฤษยุค 60s', 'เครื่องยนต์ · 400cc Parallel Twin
ระยะเวลาสร้าง · 45 วันทำการ
ระบบไอเสีย · Megaphone สเตนเลส
ยาง · Shinko คลาสสิก', 'cafe.jpg', '189000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-cafe');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-bobber', 'Dark Bobber Iron 650', 'bobber', 'สไตล์บ็อบเบอร์ทรงเตี้ย ล้อโต แฮนด์แดร็กบาร์ตรง เบาะเดี่ยวสปริง Solo Seat ตัดบังโคลนสั้นดุดันด้วยโทนสีดำด้าน Satin Black', 'เครื่องยนต์ · 650cc V-Twin Air-Cooled
ระยะเวลาสร้าง · 60 วันทำการ
ระบบไอเสีย · Shorty Ceramic ดำ
ถังน้ำมัน · Peanut Tank 2.5 Gal', 'bobber.jpg', '245000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-bobber');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-scrambler', 'Scrambler Wild Desert 500', 'scrambler', 'สายลุยผสมความเก๋า ท่อไอเสียยกสูง High-Mount ลุยน้ำลุยฝุ่นได้มั่นใจ ยางกึ่งวิบาก Dual-Sport พร้อมการ์ดกันกระแทกใต้ท้องเครื่อง', 'เครื่องยนต์ · 500cc High-Torque Single
ระยะเวลาสร้าง · 40 วันทำการ
โช้คอัพ · Sub-tank แก๊สปรับได้
แฮนด์ · Crossbar อลูมิเนียมกว้าง', 'scrambler.jpg', '195000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-scrambler');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-tracker', 'Street Flat Tracker 300', 'tracker', 'ทรงแฟลตแทร็กเกอร์สุดคล่องตัว น้ำหนักเบา ป้ายเบอร์สไตล์รถแข่ง Flat Track เฟรมท้ายขึ้นรูปใหม่ เหมาะกับขี่มุดในเมืองอย่างสนุกสนาน', 'เครื่องยนต์ · 300cc DOHC Liquid
ระยะเวลาสร้าง · 30 วันทำการ
ล้อ · ซี่ลวด 19 นิ้ว ขอบดำ
ไฟส่องสว่าง · Micro LED Projector', 'tracker.jpg', '145000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-tracker');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-brat', 'Brat Style Vintage 400', 'cafe', 'เอกลักษณ์สายเจแปนนิสคัสตอม เบาะแบนเรียบหนังแท้เย็บลายตาราง ขี่สบายสรีระคนเอเชีย แฮนด์บาร์ต่ำพอดีมือ ทรงมินิมอลไม่มีส่วนเกิน', 'เครื่องยนต์ · 400cc Single Thumper
ระยะเวลาสร้าง · 35 วันทำการ
เบาะ · หนังแท้ Diamond Stitch
เรือนไมล์ · ไมล์เดี่ยวแบบมินิมอล', 'brat.jpg', '168000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-brat');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Bike', 'bike-chopper', 'Old School Custom Chopper 750', 'chopper', 'งานระดับมาสเตอร์พีซ โช้คหน้ายืดองศายาว พนักพิงหลัง Sissy Bar ทรงสูงขึ้นรูปมือ ล้อหน้า 21 นิ้ว ถังน้ำมันเพ้นท์ลวดลายแอร์บรัชพิเศษ', 'เครื่องยนต์ · 750cc Long-Stroke
ระยะเวลาสร้าง · 90 วันทำการ
บาร์หลัง · Sissy Bar สเตนเลสแท้
ล้อหน้า · 21 นิ้ว ซี่ลวดโครเมียม', 'chopper.jpg', '320000', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='bike-chopper');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-cafe', 'แพ็กเกจแปลงโฉม Cafe Racer ครบชุด', 'CAFE RACER · CONVERSION SET', 'เปลี่ยนรถธรรมดาให้เป็นคาเฟ่เรเซอร์สุดเท่ จัดทรงหมอบซิ่งพร้อมขี่กลับบ้านทันที', 'ท่อไอเสีย Megaphone สเตนเลสเกรด 304
แฮนด์จับโช้ค Clip-on + กระจกปลายแฮนด์ CNC
เบาะตูดมดหนังแท้ Handcrafted
ฟรี! พ่นสีถังน้ำมัน 2K เคลือบเงา', 'cafe.jpg', '45900', '65900', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-cafe');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-bobber', 'ชุดแต่ง Bobber โหลดเตี้ย ล้อโต ดำด้าน', 'BOBBER · LOW & MEAN', 'อัปเกรดมิติรถให้เตี้ย ดุดัน ลุคแบดบอยสไตล์อเมริกันบ็อบเบอร์แท้ พร้อมเซ็ตระบบเบรก', 'สปริงเบาะเดี่ยว Solo Seat หนังดำดุดัน
แฮนด์แดร็กบาร์ตรง + โหลดเตี้ยโช้คหน้าหลัง
ท่อ Shorty Exhaust ยิงเลเซอร์เซรามิกดำ
ตัดแต่งบังโคลนสั้นหน้า-หลังสไตล์ Bobber', 'bobber.jpg', '32500', '47500', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-bobber');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-scrambler', 'แพ็กเกจสายลุย Scrambler All-Terrain', 'SCRAMBLER · ADVENTURE PACK', 'เซ็ตรถพร้อมลุยทั้งทางดำและทางฝุ่น ท่อไอเสียยกสูงพ้นน้ำ พร้อมการ์ดกันกระแทก', 'ยางกึ่งวิบาก Dual-Sport หน้า-หลัง
ท่อไอเสียยกสูง High-Mount สเตนเลส
โช้คอัพแก๊สพร้อมซับแทงค์ปรับหนืดได้
การ์ดกันกระแทกใต้แคร้งค์เครื่องยนต์อลูมิเนียม', 'scrambler.jpg', '28900', '36000', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-scrambler');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-paintwork', 'ทำสีถังน้ำมัน & บังโคลน แอร์บรัชพิเศษ', 'PAINT · CUSTOM AIRBRUSH', 'เพ้นท์ลวดลายเฉพาะคันตามสั่ง ลายเฟลม ลายตาราง ลายกราฟิกเรโทร ด้วยสี 2K ทนทาน', 'จำลองลวดลาย 3D ให้ตรวจก่อนลงมือพ่นจริง
สีเคลือบใส 2K Ceramic Clear เงาฉ่ำกันรอยขีดข่วน
รับประกันงานสีไม่ลอกร่อนนาน 2 ปี
ฟรีบริการเคลือบเซรามิกแท้ 1 รอบ', 'paintwork.jpg', '6900', '9900', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-paintwork');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-tuning', 'ชุดปลายท่อคัสตอม + จูนคาร์บู/กล่องฟรี', 'PERFORMANCE · EXHAUST TUNING', 'เพิ่มแรงม้าและแรงบิด อัตราเร่งดีขึ้นเห็นผลชัดเจน เสียงแน่นกระหึ่มถูกใจ', 'ปลายท่อ Megaphone หรือ Shorty ตามสั่ง
เซ็ตติ้งคาร์บูเรเตอร์หรือจูนกล่อง ECU บนแท่น Dyno
แถมฟรี กรองเปลือยคลาสสิก High-Flow
วัดแรงม้าก่อนและหลังติดตั้งพร้อมกราฟผลการทดสอบ', 'tuning.jpg', '8500', '12000', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-tuning');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'package-voucher', 'สิทธิ์สมาชิก สั่งประกอบรับ Voucher อะไหล่', 'PRIVILEGE · MEMBER ONLY', 'สำหรับสมาชิก CustomShop เมื่อสั่งประกอบรถคัสตอม รับคูปองเลือกช้อปอะไหล่และหมวก', 'Gift Voucher อะไหล่และหมวกกันน็อก ฿10,000
ฟรีค่าแรงตรวจเช็คระยะ 4 ครั้งตลอดปีแรก
บริการช่วยเหลือรถสไลด์ฉุกเฉิน 24 ชม. ทั่วกรุงเทพฯ
ผ่อนชำระ 0% สูงสุด 10 เดือนผ่านบัตรเครดิต', 'voucher.jpg', '0', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='package-voucher');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Package', 'season-build', 'สั่งประกอบ Custom Bike ทั้งคัน ลด 15%', 'Custom Build', 'ขอประเมินค่าประกอบรถคัสตอม พร้อมระบุรายละเอียดกับช่าง', 'ส่วนลดค่าประกอบ 15%', 'hero.jpg', '0', '0', TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='season-build');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product','product-brass','ไฟหน้า LED Retro Brass 7"','parts','โคมกระจกเลนส์นูนคลาสสิก พร้อมขอบวงแหวนทองเหลืองแท้ปัดเงา ลำแสงสว่างคมชัด','','brass.jpg',2890,0,TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-brass');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-BRASS',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-brass' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-BRASS');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product','product-flat-seat','เบาะหนังแท้ Tuck & Roll','parts','หนังอิตาลีฟอกฝาดสีน้ำตาลวินเทจ เย็บลอนขวางสุดประณีต ฟองน้ำฉีดขึ้นรูปทรงตัว','','flat-seat.jpg',4500,0,TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-flat-seat');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-FLAT-SEAT',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-flat-seat' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-FLAT-SEAT');

INSERT INTO shop_catalog (Kind,Slug,Name,Category,Description,Features,Image,Price,OriginalPrice,Active)
SELECT 'Product','product-tank','ถังน้ำมันทรงวินเทจ Pinstripe','parts','เหล็กหนาพิเศษเคลือบกันสนิมภายใน พ่นสี 2K ฉ่ำลึก ลากเส้นพู่กันด้วยมือทีละคัน','','tank.jpg',8900,0,TRUE WHERE NOT EXISTS (SELECT 1 FROM shop_catalog WHERE Slug='product-tank');
INSERT INTO shop_stock (CatalogItemId,ItemCode,ItemName,Category,Size,QuantityOnHand,MinRequired,Unit,Location)
SELECT Id,'PRODUCT-TANK',Name,Category,'มาตรฐาน',0,1,'ชิ้น','' FROM shop_catalog WHERE Slug='product-tank' AND NOT EXISTS (SELECT 1 FROM shop_stock WHERE ItemCode='PRODUCT-TANK');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'CUSTOMSEASON15',Id FROM shop_catalog WHERE Slug='season-build' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='CUSTOMSEASON15');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'CAFERACER20K',Id FROM shop_catalog WHERE Slug='package-cafe' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='CAFERACER20K');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'BOBBERLOW15',Id FROM shop_catalog WHERE Slug='package-bobber' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='BOBBERLOW15');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'SCRAMBLERFREE',Id FROM shop_catalog WHERE Slug='package-scrambler' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='SCRAMBLERFREE');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'PAINTART30',Id FROM shop_catalog WHERE Slug='package-paintwork' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='PAINTART30');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'EXHAUSTBOOST',Id FROM shop_catalog WHERE Slug='package-tuning' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='EXHAUSTBOOST');
INSERT INTO shop_package_codes (Code,CatalogItemId) SELECT 'MEMBERVIP',Id FROM shop_catalog WHERE Slug='package-voucher' AND NOT EXISTS (SELECT 1 FROM shop_package_codes WHERE Code='MEMBERVIP');
COMMIT;
