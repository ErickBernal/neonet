
CREATE TABLE Productos (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  NVARCHAR(200)  NOT NULL,
    Precio  DECIMAL(10,2)  NOT NULL,
    Stock   INT            NOT NULL
);

CREATE TABLE Clientes (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  NVARCHAR(200)  NOT NULL,
    Email   NVARCHAR(200)  NOT NULL
);

CREATE TABLE Ventas (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    Fecha      DATETIME NOT NULL DEFAULT GETDATE(),
    ClienteId  INT      NOT NULL,

    CONSTRAINT FK_Ventas_Clientes
        FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE DetalleVenta (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    VentaId         INT           NOT NULL,
    ProductoId      INT           NOT NULL,
    Cantidad        INT           NOT NULL,
    PrecioUnitario  DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_DetalleVenta_Ventas
        FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
    CONSTRAINT FK_DetalleVenta_Productos
        FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
