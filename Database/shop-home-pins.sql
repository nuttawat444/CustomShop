CREATE TABLE IF NOT EXISTS shop_home_pins (
    CatalogItemId INT NOT NULL PRIMARY KEY,
    IsPinned BOOLEAN NOT NULL,
    PinnedAt DATETIME(6) NOT NULL,
    CONSTRAINT FK_shop_home_pins_catalog FOREIGN KEY (CatalogItemId)
        REFERENCES shop_catalog(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Initial entries match the existing Home cards. Unpinned entries are kept, so repeat runs
-- cannot re-pin items after an administrator changes the selection.
START TRANSACTION;
SET @home_pins_empty := NOT EXISTS (SELECT 1 FROM shop_home_pins);
INSERT INTO shop_home_pins (CatalogItemId, IsPinned, PinnedAt)
SELECT slots.Id, TRUE, TIMESTAMPADD(MICROSECOND, -CAST(slots.DisplayRank AS SIGNED), UTC_TIMESTAMP(6))
FROM (
    SELECT Id, ROW_NUMBER() OVER (
        PARTITION BY Kind ORDER BY CASE WHEN Kind = 'Bike' THEN Id ELSE -Id END
    ) AS DisplayRank
    FROM shop_catalog
    WHERE Active = TRUE AND (Kind = 'Bike' OR (Kind = 'Product' AND Category <> 'helmet'))
) AS slots
WHERE @home_pins_empty AND slots.DisplayRank <= 4
ON DUPLICATE KEY UPDATE IsPinned = shop_home_pins.IsPinned;
COMMIT;
