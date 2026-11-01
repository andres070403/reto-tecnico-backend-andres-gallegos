
/* =============================================================
   Reto Técnico Fullstack Senior - Gestión de Pedidos
   Script de creación de base de datos (SQL Server)
   ============================================================= */

-- 1. Base de datos ------------------------------------------------
IF DB_ID('PedidosDb') IS NULL
    CREATE DATABASE PedidosDb;
GO

USE PedidosDb;
GO

-- 2. Tabla Usuarios -----------------------------------------------
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id              INT IDENTITY(1,1) NOT NULL,
        Nombre          NVARCHAR(150)     NOT NULL,
        Email           NVARCHAR(150)     NOT NULL,
        PasswordHash    NVARCHAR(255)     NOT NULL, 
        Rol             NVARCHAR(20)      NOT NULL,
        Activo          BIT               NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
        FechaCreacion   DATETIME          NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT (GETDATE()),

        CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
        CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
        CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Admin', 'User'))
    );
END
GO

-- 3. Tabla Pedidos ------------------------------------------------
IF OBJECT_ID('dbo.Pedidos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos (
        Id                  INT IDENTITY(1,1) NOT NULL,
        NumeroPedido        VARCHAR(50)       NOT NULL,
        Cliente             VARCHAR(150)      NOT NULL,
        Fecha               DATETIME          NOT NULL,
        Total               DECIMAL(10,2)     NOT NULL,
        Estado              VARCHAR(50)       NOT NULL,
        Eliminado           BIT               NOT NULL CONSTRAINT DF_Pedidos_Eliminado DEFAULT (0),
        UsuarioCreacionId       INT           NOT NULL,
        FechaCreacion           DATETIME     NOT NULL CONSTRAINT DF_Pedidos_FechaCreacion DEFAULT (GETDATE()),
        UsuarioModificacionId   INT           NULL,
        FechaActualizacion      DATETIME     NULL,

        CONSTRAINT PK_Pedidos PRIMARY KEY (Id),
        CONSTRAINT FK_Pedidos_UsuarioCreacion     FOREIGN KEY (UsuarioCreacionId)     REFERENCES dbo.Usuarios (Id),
        CONSTRAINT FK_Pedidos_UsuarioModificacion FOREIGN KEY (UsuarioModificacionId) REFERENCES dbo.Usuarios (Id),
        CONSTRAINT UQ_Pedidos_NumeroPedido UNIQUE (NumeroPedido),                -- Regla: número único
        CONSTRAINT CK_Pedidos_Total CHECK (Total > 0),                           -- Regla: total > 0
        CONSTRAINT CK_Pedidos_Estado CHECK (Estado IN ('Registrado', 'EnProceso', 'Completado', 'Cancelado'))
    );

END
GO

-- 4. Datos de prueba ----------------------------------------------
-- Credenciales:
--   admin@pedidos.com / Admin123*   (rol Admin)
--   user@pedidos.com  / User123*    (rol User)
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios)
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Email, PasswordHash, Rol) VALUES
    ('Administrador', 'admin@pedidos.com', '$2a$11$6/hvar.9e3218SzBYSruKe135GDRXVLzVtO08r8t0RnYUNvd1kpjm', 'Admin'),
    ('Usuario Demo',  'user@pedidos.com',  '$2a$11$weSJCsNw0/hk1O0jb3YTI.FHbLjUUoDPM3j6ZDZr6d3De3j2.GHXC', 'User');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos)
BEGIN
    DECLARE @AdminId INT = (SELECT Id FROM dbo.Usuarios WHERE Email = 'admin@pedidos.com');
    DECLARE @UserId  INT = (SELECT Id FROM dbo.Usuarios WHERE Email = 'user@pedidos.com');

 INSERT INTO dbo.Pedidos (NumeroPedido, Cliente, Fecha, Total, Estado, UsuarioCreacionId) VALUES
('PE-001', 'Juan Perez',      '20250110', 250.75,  'Registrado', @AdminId),
('PE-002', 'Maria Gonzalez',  '20250112', 1200.00, 'EnProceso',  @AdminId),
('PE-003', 'Carlos Ramirez',  '20250115', 89.90,   'Completado', @UserId),
('PE-004', 'Ana Torres',      '20250118', 430.50,  'Registrado', @UserId),
('PE-005', 'Luis Fernandez',  '20250120', 75.00,   'Cancelado',  @AdminId);
END
GO