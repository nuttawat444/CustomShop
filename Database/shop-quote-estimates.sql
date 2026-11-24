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
