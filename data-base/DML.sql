
INSERT INTO Productos (Nombre, Precio, Stock) VALUES
('Teclado mecánico',    45.99, 20),
('Mouse inalámbrico',   19.99, 50),
('Monitor 24 pulgadas', 150.00, 10),
('Audífonos bluetooth', 35.50, 30),
('Webcam HD',           25.00, 15);

INSERT INTO Clientes (Nombre, Email) VALUES
('Ana Gómez',       'ana.gomez@correo.com'),
('Carlos Pérez',    'carlos.perez@correo.com'),
('Lucía Fernández', 'lucia.fernandez@correo.com');

INSERT INTO Ventas (Fecha, ClienteId) VALUES ('2026-08-10', 1);
INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario) VALUES
(1, 1, 1, 45.99),
(1, 2, 2, 19.99);

INSERT INTO Ventas (Fecha, ClienteId) VALUES ('2026-08-12', 2);
INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario) VALUES
(2, 3, 1, 150.00);

INSERT INTO Ventas (Fecha, ClienteId) VALUES ('2026-08-15', 1);
INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario) VALUES
(3, 4, 2, 35.50),
(3, 5, 1, 25.00);

INSERT INTO Ventas (Fecha, ClienteId) VALUES ('2026-08-16', 3);
INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario) VALUES
(4, 2, 3, 19.99),
(4, 4, 1, 35.50);
