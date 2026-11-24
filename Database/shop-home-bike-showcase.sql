-- Add the four recommended bikes below the existing Home selections.
-- Existing pin settings are preserved, including items manually unpinned by an administrator.
START TRANSACTION;

INSERT INTO shop_home_pins (CatalogItemId, IsPinned, PinnedAt)
SELECT bike.Id, TRUE, TIMESTAMPADD(MICROSECOND, -pick.SortOrder, existing.OldestPin)
FROM (
    SELECT 'bike-cafe-silverline-400' AS Slug, 1 AS SortOrder
    UNION ALL SELECT 'bike-scrambler-fieldstone-500', 2
    UNION ALL SELECT 'bike-brat-after-hours-400', 3
    UNION ALL SELECT 'bike-chopper-horizon-750', 4
) pick
JOIN shop_catalog bike ON bike.Slug = pick.Slug AND bike.Kind = 'Bike' AND bike.Active
CROSS JOIN (
    SELECT COALESCE(MIN(pin.PinnedAt), UTC_TIMESTAMP(6)) AS OldestPin
    FROM shop_home_pins pin
    JOIN shop_catalog item ON item.Id = pin.CatalogItemId
    WHERE item.Kind = 'Bike' AND item.Active AND pin.IsPinned
) existing
WHERE NOT EXISTS (SELECT 1 FROM shop_home_pins pin WHERE pin.CatalogItemId = bike.Id)
ON DUPLICATE KEY UPDATE CatalogItemId = shop_home_pins.CatalogItemId;

COMMIT;
