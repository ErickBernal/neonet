-- 1) Total vendido por cliente
SELECT
    c.Id            AS ClienteId,
    c.Nombre        AS Cliente,
    SUM(dv.Cantidad * dv.PrecioUnitario) AS TotalVendido
FROM Clientes c
JOIN Ventas v        ON v.ClienteId = c.Id
JOIN DetalleVenta dv ON dv.VentaId  = v.Id
GROUP BY c.Id, c.Nombre
ORDER BY TotalVendido DESC;


-- 2) Productos más vendidos (por cantidad de unidades)
SELECT
    p.Id                   AS ProductoId,
    p.Nombre                AS Producto,
    SUM(dv.Cantidad)        AS UnidadesVendidas,
    SUM(dv.Cantidad * dv.PrecioUnitario) AS TotalGenerado
FROM Productos p
JOIN DetalleVenta dv ON dv.ProductoId = p.Id
GROUP BY p.Id, p.Nombre
ORDER BY UnidadesVendidas DESC;


-- 3) Stock actual
SELECT
    Id,
    Nombre,
    Stock
FROM Productos
ORDER BY Nombre;
