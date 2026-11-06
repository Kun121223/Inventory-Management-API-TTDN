SELECT
    COALESCE(SUM(Quantity * UnitPrice), 0) AS TotalStockValue
FROM dbo.Items;