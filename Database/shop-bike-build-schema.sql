CREATE TABLE IF NOT EXISTS shop_bike_part_slots (
    BikeId INT NOT NULL,
    SlotCode VARCHAR(40) NOT NULL,
    SlotName VARCHAR(120) NOT NULL,
    SortOrder INT NOT NULL,
    MinQuantity INT NOT NULL,
    MaxQuantity INT NOT NULL,
    DefaultStockItemId INT NULL,
    PRIMARY KEY (BikeId, SlotCode),
    CONSTRAINT FK_bike_slots_bike FOREIGN KEY (BikeId) REFERENCES shop_catalog(Id),
    CONSTRAINT FK_bike_slots_default FOREIGN KEY (DefaultStockItemId) REFERENCES shop_stock(Id),
    CONSTRAINT CK_bike_slot_quantity CHECK (MinQuantity >= 0 AND MaxQuantity >= MinQuantity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS shop_bike_part_options (
    BikeId INT NOT NULL,
    SlotCode VARCHAR(40) NOT NULL,
    StockItemId INT NOT NULL,
    PRIMARY KEY (BikeId, SlotCode, StockItemId),
    CONSTRAINT FK_bike_options_slot FOREIGN KEY (BikeId, SlotCode)
        REFERENCES shop_bike_part_slots(BikeId, SlotCode) ON DELETE CASCADE,
    CONSTRAINT FK_bike_options_stock FOREIGN KEY (StockItemId) REFERENCES shop_stock(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS shop_project_parts (
    ProjectCode VARCHAR(50) NOT NULL,
    SlotCode VARCHAR(40) NOT NULL,
    SlotName VARCHAR(120) NOT NULL,
    StockItemId INT NOT NULL,
    Sku VARCHAR(100) NOT NULL,
    PartName VARCHAR(220) NOT NULL,
    Variant VARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(14,2) NOT NULL,
    PRIMARY KEY (ProjectCode, SlotCode),
    CONSTRAINT FK_project_parts_project FOREIGN KEY (ProjectCode) REFERENCES shop_projects(ProjectCode) ON DELETE CASCADE,
    CONSTRAINT FK_project_parts_stock FOREIGN KEY (StockItemId) REFERENCES shop_stock(Id),
    CONSTRAINT CK_project_parts_quantity CHECK (Quantity > 0 AND UnitPrice >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
