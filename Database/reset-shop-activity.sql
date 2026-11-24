-- CustomShop activity reset for MySQL. Run the whole file in a dedicated connection
-- while the app is stopped. Keep foreign key checks enabled.
-- Preview: run this file as-is in the intended database.
-- Apply: in the same connection, set @customshop_reset_confirm = 'RESET csi402db';
--        then run this whole file. Replace csi402db with the selected database name.
-- The confirmation is consumed on each run. A successful apply cannot be undone.

SET @reset_requested = COALESCE(BINARY @customshop_reset_confirm = BINARY CONCAT('RESET ', DATABASE()), FALSE);
SET @customshop_reset_confirm = NULL;
SET @reset_previous_safe_updates = @@SESSION.sql_safe_updates;

SELECT COUNT(*) = 11 AND COALESCE(SUM(ENGINE = 'InnoDB'), 0) = 11
INTO @reset_transactional
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME IN (
    'shop_warehouse_tasks', 'shop_payments', 'shop_invoices', 'shop_order_lines', 'shop_orders',
    'shop_quote_estimate_lines', 'shop_quote_estimates', 'shop_job_assignees', 'shop_jobs',
    'shop_project_parts', 'shop_projects'
);

SET @reset_apply = @reset_requested AND @reset_transactional AND @@SESSION.foreign_key_checks = 1;
SET SESSION sql_safe_updates = 0;

START TRANSACTION;

SELECT DATABASE() AS DatabaseName,
    CASE WHEN @reset_apply THEN 'APPLY' WHEN @reset_requested THEN 'BLOCKED' ELSE 'PREVIEW' END AS ResetMode;

SELECT 'shop_orders' AS TableName, COUNT(*) AS RowsInScope FROM shop_orders
UNION ALL SELECT 'shop_order_lines', COUNT(*) FROM shop_order_lines
UNION ALL SELECT 'shop_payments', COUNT(*) FROM shop_payments
UNION ALL SELECT 'shop_invoices', COUNT(*) FROM shop_invoices
UNION ALL SELECT 'shop_warehouse_tasks (linked to orders)', COUNT(*) FROM shop_warehouse_tasks WHERE SaleOrderId IS NOT NULL
UNION ALL SELECT 'shop_projects', COUNT(*) FROM shop_projects
UNION ALL SELECT 'shop_project_parts', COUNT(*) FROM shop_project_parts
UNION ALL SELECT 'shop_quote_estimates', COUNT(*) FROM shop_quote_estimates
UNION ALL SELECT 'shop_quote_estimate_lines', COUNT(*) FROM shop_quote_estimate_lines
UNION ALL SELECT 'shop_jobs', COUNT(*) FROM shop_jobs
UNION ALL SELECT 'shop_job_assignees', COUNT(*) FROM shop_job_assignees;

-- Delete child records before their parents. Standalone warehouse tasks are kept.
DELETE FROM shop_warehouse_tasks WHERE @reset_apply AND SaleOrderId IS NOT NULL;
DELETE FROM shop_payments WHERE @reset_apply;
DELETE FROM shop_invoices WHERE @reset_apply;
DELETE FROM shop_order_lines WHERE @reset_apply;
DELETE FROM shop_orders WHERE @reset_apply;

DELETE FROM shop_quote_estimate_lines WHERE @reset_apply;
DELETE FROM shop_quote_estimates WHERE @reset_apply;
DELETE FROM shop_job_assignees WHERE @reset_apply;
DELETE FROM shop_jobs WHERE @reset_apply;
DELETE FROM shop_project_parts WHERE @reset_apply;
DELETE FROM shop_projects WHERE @reset_apply;

SET @reset_empty =
    NOT EXISTS (SELECT 1 FROM shop_warehouse_tasks WHERE SaleOrderId IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM shop_payments)
    AND NOT EXISTS (SELECT 1 FROM shop_invoices)
    AND NOT EXISTS (SELECT 1 FROM shop_order_lines)
    AND NOT EXISTS (SELECT 1 FROM shop_orders)
    AND NOT EXISTS (SELECT 1 FROM shop_quote_estimate_lines)
    AND NOT EXISTS (SELECT 1 FROM shop_quote_estimates)
    AND NOT EXISTS (SELECT 1 FROM shop_job_assignees)
    AND NOT EXISTS (SELECT 1 FROM shop_jobs)
    AND NOT EXISTS (SELECT 1 FROM shop_project_parts)
    AND NOT EXISTS (SELECT 1 FROM shop_projects);

-- Roll back if any target rows remain, including when a client continued after a deletion error.
SET @reset_finish_sql = IF(@reset_apply AND @reset_empty, 'COMMIT', 'ROLLBACK');
PREPARE customshop_reset_finish FROM @reset_finish_sql;
EXECUTE customshop_reset_finish;
DEALLOCATE PREPARE customshop_reset_finish;

SET SESSION sql_safe_updates = @reset_previous_safe_updates;
SELECT CASE
    WHEN @reset_apply AND @reset_empty THEN 'CLEARED'
    WHEN @reset_apply THEN 'ROLLED_BACK'
    WHEN @reset_requested THEN 'BLOCKED: check database engines and foreign key checks'
    ELSE 'PREVIEW_ONLY'
END AS ResetResult;

SET @reset_requested = NULL, @reset_previous_safe_updates = NULL, @reset_transactional = NULL,
    @reset_apply = NULL, @reset_empty = NULL, @reset_finish_sql = NULL;
