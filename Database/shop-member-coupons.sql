-- Existing coupons with the same code retain their configured terms.
INSERT INTO shop_coupons (Code, Category, Percent, Active) VALUES
    ('SHOP03', '*', 3, TRUE),
    ('SHOP05', '*', 5, TRUE),
    ('SHOP07', '*', 7, TRUE),
    ('SHOP10', '*', 10, TRUE),
    ('HELMET08', 'helmet', 8, TRUE),
    ('HELMET12', 'helmet', 12, TRUE),
    ('PARTS08', 'parts', 8, TRUE),
    ('PARTS12', 'parts', 12, TRUE),
    ('EXHAUST08', 'exhaust', 8, TRUE),
    ('EXHAUST12', 'exhaust', 12, TRUE)
ON DUPLICATE KEY UPDATE Code = shop_coupons.Code;
