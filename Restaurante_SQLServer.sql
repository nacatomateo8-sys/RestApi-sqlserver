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

CREATE TABLE Mesas (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Numero    INT NOT NULL UNIQUE,
    Capacidad INT NOT NULL CHECK (Capacidad > 0)
);
GO

CREATE TABLE Usuarios (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Nombre       NVARCHAR(100) NOT NULL,
    Correo       NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Rol          NVARCHAR(50)  NOT NULL,
    Activo       BIT           NOT NULL DEFAULT 1
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

INSERT INTO Mesas (Numero, Capacidad) VALUES
 (1, 4), (2, 2), (3, 6), (4, 4);

-- Usuario inicial administrador para pruebas (admin@itq.ec / Admin2026*)
INSERT INTO Usuarios (Nombre, Correo, PasswordHash, Rol, Activo) VALUES
 (N'Admin ITQ', N'admin@itq.ec', N'$2a$11$Qe7ye3/2aYqzqNj1tgYbweWvB7ZtZ9CCgCT2FlGZ89867Dglfyvyu', N'Administrador', 1);
GO
