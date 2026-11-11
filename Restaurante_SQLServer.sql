-- Script para SQL Server (SSMS)
IF DB_ID('Restaurante') IS NULL CREATE DATABASE Restaurante;
GO
USE Restaurante;
GO

CREATE TABLE Categorias (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE Platos (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio      DECIMAL(10,2) NOT NULL CHECK (Precio > 0),
    CategoriaId INT NOT NULL,
    Disponible  BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Platos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);
GO

-- Datos de prueba
INSERT INTO Categorias (Nombre) VALUES (N'Entradas'), (N'Platos fuertes'), (N'Postres'), (N'Bebidas');

INSERT INTO Platos (Nombre, Descripcion, Precio, CategoriaId) VALUES
 (N'Ceviche de camarón', N'Camarón, cebolla y limón', 8.50, 1),
 (N'Encebollado', N'Atún, yuca y cebolla curtida', 5.00, 2),
 (N'Hornado', N'Cerdo con mote y llapingachos', 9.75, 2),
 (N'Helado de paila', N'Postre tradicional', 2.50, 3),
 (N'Jugo de naranjilla', N'Natural', 2.00, 4);
GO
