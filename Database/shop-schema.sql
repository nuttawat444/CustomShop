-- Additive MySQL schema for the CustomShop application.

CREATE TABLE IF NOT EXISTS shop_catalog (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Kind VARCHAR(20) NOT NULL,
    Slug VARCHAR(100) NOT NULL,
    Name VARCHAR(220) NOT NULL,
    Category VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    Features TEXT NOT NULL,
    Image VARCHAR(120) NOT NULL,
    Price DECIMAL(14,2) NOT NULL,
    OriginalPrice DECIMAL(14,2) NOT NULL,
    Active BOOLEAN NOT NULL,
    UNIQUE (Slug),
    CHECK (Price >= 0 AND OriginalPrice >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_stock (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    CatalogItemId INT NULL,
    ItemCode VARCHAR(100) NOT NULL,
    ItemName VARCHAR(220) NOT NULL,
    Category VARCHAR(100) NOT NULL,
    Size VARCHAR(20) NOT NULL,
    QuantityOnHand INT NOT NULL,
    MinRequired INT NOT NULL,
    Unit VARCHAR(30) NOT NULL,
    Location VARCHAR(80) NOT NULL,
    UNIQUE (ItemCode),
    UNIQUE (CatalogItemId,Size),
    FOREIGN KEY (CatalogItemId) REFERENCES shop_catalog(Id),
    CHECK (QuantityOnHand >= 0 AND MinRequired >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_carts (
    UserId INT NOT NULL PRIMARY KEY,
    Revision VARCHAR(32) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_cart_lines (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    StockItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UNIQUE (UserId,StockItemId),
    FOREIGN KEY (UserId) REFERENCES shop_carts(UserId) ON DELETE CASCADE,
    FOREIGN KEY (StockItemId) REFERENCES shop_stock(Id),
    CHECK (Quantity BETWEEN 1 AND 999)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_coupons (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(40) NOT NULL,
    Category VARCHAR(100) NOT NULL,
    Percent DECIMAL(14,2) NOT NULL,
    Active BOOLEAN NOT NULL,
    UNIQUE (Code),
    CHECK (Percent > 0 AND Percent <= 100)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_orders (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Revision VARCHAR(32) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    Fulfillment VARCHAR(20) NOT NULL,
    Address VARCHAR(500) NOT NULL,
    CouponCode VARCHAR(40) NOT NULL,
    Subtotal DECIMAL(14,2) NOT NULL,
    Discount DECIMAL(14,2) NOT NULL,
    Total DECIMAL(14,2) NOT NULL,
    UNIQUE (UserId,Revision),
    FOREIGN KEY (UserId) REFERENCES users(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_order_lines (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    SaleOrderId INT NOT NULL,
    StockItemId INT NOT NULL,
    Name VARCHAR(250) NOT NULL,
    Size VARCHAR(20) NOT NULL,
    UnitPrice DECIMAL(14,2) NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (SaleOrderId) REFERENCES shop_orders(Id),
    FOREIGN KEY (StockItemId) REFERENCES shop_stock(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_payments (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    SaleOrderId INT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    Amount DECIMAL(14,2) NOT NULL,
    Success BOOLEAN NOT NULL,
    UNIQUE (SaleOrderId),
    FOREIGN KEY (UserId) REFERENCES users(id),
    FOREIGN KEY (SaleOrderId) REFERENCES shop_orders(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_invoices (
    InvoiceNo VARCHAR(50) NOT NULL PRIMARY KEY,
    SaleOrderId INT NULL,
    CustomerName VARCHAR(220) NOT NULL,
    TotalAmount DECIMAL(14,2) NOT NULL,
    DiscountAmount DECIMAL(14,2) NOT NULL,
    PaymentMethod VARCHAR(100) NOT NULL,
    Status VARCHAR(80) NOT NULL,
    PaidAt DATETIME(6) NULL,
    CreatedAt DATETIME(6) NOT NULL,
    ItemDescription TEXT NOT NULL,
    UNIQUE (SaleOrderId),
    FOREIGN KEY (SaleOrderId) REFERENCES shop_orders(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_warehouse_tasks (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    SaleOrderId INT NULL,
    TaskTitle VARCHAR(800) NOT NULL,
    DueTime VARCHAR(80) NOT NULL,
    IsCompleted BOOLEAN NOT NULL,
    CompletedAt DATETIME(6) NULL,
    UNIQUE (SaleOrderId),
    FOREIGN KEY (SaleOrderId) REFERENCES shop_orders(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_projects (
    ProjectCode VARCHAR(50) NOT NULL PRIMARY KEY,
    CatalogItemId INT NULL,
    UserId INT NULL,
    ProjectName VARCHAR(220) NOT NULL,
    CustomerName VARCHAR(220) NOT NULL,
    Budget DECIMAL(14,2) NOT NULL,
    Status VARCHAR(100) NOT NULL,
    ProgressPercentage INT NOT NULL,
    IsApproved BOOLEAN NOT NULL,
    FOREIGN KEY (CatalogItemId) REFERENCES shop_catalog(Id),
    FOREIGN KEY (UserId) REFERENCES users(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_jobs (
    JobTicketNo VARCHAR(50) NOT NULL PRIMARY KEY,
    ProjectCode VARCHAR(50) NULL,
    BikeModel VARCHAR(220) NOT NULL,
    CustomStyle VARCHAR(100) NOT NULL,
    ServiceType VARCHAR(500) NOT NULL,
    AssignedMechanic VARCHAR(220) NOT NULL,
    Priority VARCHAR(20) NOT NULL DEFAULT '',
    Status VARCHAR(100) NOT NULL,
    ProgressPercent INT NOT NULL,
    CompletedAt DATETIME(6) NULL,
    UNIQUE (ProjectCode),
    FOREIGN KEY (ProjectCode) REFERENCES shop_projects(ProjectCode)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_package_codes (
    Code VARCHAR(40) PRIMARY KEY,
    CatalogItemId INT NOT NULL UNIQUE,
    FOREIGN KEY (CatalogItemId) REFERENCES shop_catalog(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_saved_offers (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CouponId INT NULL,
    PackageCode VARCHAR(40) NULL,
    UNIQUE (UserId, CouponId),
    UNIQUE (UserId, PackageCode),
    CHECK ((CouponId IS NULL) <> (PackageCode IS NULL)),
    FOREIGN KEY (UserId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (CouponId) REFERENCES shop_coupons(Id),
    FOREIGN KEY (PackageCode) REFERENCES shop_package_codes(Code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS shop_job_assignees (
    JobTicketNo VARCHAR(50) NOT NULL PRIMARY KEY,
    MechanicUserId INT NOT NULL,
    KEY IX_shop_job_assignees_mechanic (MechanicUserId),
    CONSTRAINT FK_shop_job_assignees_job FOREIGN KEY (JobTicketNo)
        REFERENCES shop_jobs(JobTicketNo) ON DELETE CASCADE,
    CONSTRAINT FK_shop_job_assignees_user FOREIGN KEY (MechanicUserId)
        REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS shop_home_pins (
    CatalogItemId INT NOT NULL PRIMARY KEY,
    IsPinned BOOLEAN NOT NULL,
    PinnedAt DATETIME(6) NOT NULL,
    CONSTRAINT FK_shop_home_pins_catalog FOREIGN KEY (CatalogItemId)
        REFERENCES shop_catalog(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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

CREATE TABLE IF NOT EXISTS shop_quote_estimates (
    ProjectCode VARCHAR(50) NOT NULL,
    Revision INT NOT NULL,
    EditVersion INT NOT NULL,
    LaborAmount DECIMAL(14,2) NOT NULL,
    OtherAmount DECIMAL(14,2) NOT NULL,
    DiscountAmount DECIMAL(14,2) NOT NULL,
    TotalAmount DECIMAL(14,2) NOT NULL,
    Notes VARCHAR(1000) NOT NULL,
    UpdatedBy INT NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    SubmittedAt DATETIME(6) NULL,
    ReviewedBy INT NULL,
    ReviewedAt DATETIME(6) NULL,
    ReturnReason VARCHAR(1000) NULL,
    PRIMARY KEY (ProjectCode, Revision),
    CONSTRAINT FK_estimate_project FOREIGN KEY (ProjectCode) REFERENCES shop_projects(ProjectCode),
    CONSTRAINT FK_estimate_accountant FOREIGN KEY (UpdatedBy) REFERENCES users(id),
    CONSTRAINT FK_estimate_reviewer FOREIGN KEY (ReviewedBy) REFERENCES users(id),
    CONSTRAINT CK_estimate_revision CHECK (Revision > 0 AND EditVersion > 0),
    CONSTRAINT CK_estimate_amounts CHECK (
        LaborAmount BETWEEN 0 AND 10000000 AND OtherAmount BETWEEN 0 AND 10000000
        AND DiscountAmount BETWEEN 0 AND 10000000 AND TotalAmount BETWEEN 0 AND 10000000
    ),
    CONSTRAINT CK_estimate_review CHECK (
        (ReviewedAt IS NULL AND ReviewedBy IS NULL AND ReturnReason IS NULL)
        OR (SubmittedAt IS NOT NULL AND ReviewedAt IS NOT NULL AND ReviewedBy IS NOT NULL)
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS shop_quote_estimate_lines (
    ProjectCode VARCHAR(50) NOT NULL,
    Revision INT NOT NULL,
    SlotCode VARCHAR(40) NOT NULL,
    PartName VARCHAR(220) NOT NULL,
    Sku VARCHAR(100) NOT NULL,
    Variant VARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(14,2) NOT NULL,
    PRIMARY KEY (ProjectCode, Revision, SlotCode),
    CONSTRAINT FK_estimate_line_revision FOREIGN KEY (ProjectCode, Revision)
        REFERENCES shop_quote_estimates(ProjectCode, Revision),
    CONSTRAINT CK_estimate_line_amount CHECK (Quantity > 0 AND UnitPrice BETWEEN 0 AND 10000000)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
