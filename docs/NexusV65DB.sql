-- ============================================================================
-- SCRIPT DE BASE DE DATOS - NEXUS OS v6.5 (HITO 4)
-- Motor de Juego: Sistema de Persistencia Relacional SQL Server
-- ============================================================================

CREATE DATABASE NexusV65DB
GO

USE NexusV65DB
GO

-- 1. TABLA: Cadetes
CREATE TABLE Cadetes (
    CadeteId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Rango INT NOT NULL DEFAULT 1,
    Experiencia INT NOT NULL DEFAULT 0,
    VidaActual INT NOT NULL DEFAULT 100,
    VidaMax INT NOT NULL DEFAULT 100,
    EnergiaCore INT NOT NULL DEFAULT 35,
    FechaUltimoAcceso DATETIME DEFAULT GETDATE()
);
GO

-- 2. TABLA: InventarioCadete
CREATE TABLE InventarioCadete (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    CadeteId INT NOT NULL,
    NombreItem VARCHAR(50) NOT NULL,
    TipoItem VARCHAR(20) NOT NULL,
    Valor INT NOT NULL,
    Icono VARCHAR(10) NOT NULL,
    FOREIGN KEY (CadeteId) REFERENCES Cadetes(CadeteId) ON DELETE CASCADE
);
GO

-- 3. TABLA: BitacoraEventos
CREATE TABLE BitacoraEventos (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    CadeteId INT NOT NULL,
    Mensaje VARCHAR(255) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CadeteId) REFERENCES Cadetes(CadeteId) ON DELETE CASCADE
);
GO

-- 4. TABLA: Naves
CREATE TABLE Naves (
    NaveId INT IDENTITY(1,1) PRIMARY KEY,
    NombreNave VARCHAR(50) NOT NULL,
    Combustible INT NOT NULL CHECK (Combustible BETWEEN 0 AND 100),
    SaludEscudo INT NOT NULL CHECK (SaludEscudo BETWEEN 0 AND 100),
    CapacidadCarga INT NOT NULL
);
GO

-- DATOS DE PRUEBA INICIALES
INSERT INTO Cadetes (Nombre, Rango, Experiencia, VidaActual, VidaMax, EnergiaCore) 
VALUES ('Cadete NEXUS', 1, 0, 100, 100, 35);

INSERT INTO InventarioCadete (CadeteId, NombreItem, TipoItem, Valor, Icono) 
VALUES (1, 'Célula de energía 🔋', 'Energia', 25, '🔋'),
       (1, 'Escudo de Plasma 🛡️', 'Escudo', 30, '🛡️');

INSERT INTO Naves (NombreNave, Combustible, SaludEscudo, CapacidadCarga)
VALUES ('NEXUS-Vanguard', 100, 80, 50),
       ('Goliath-Cargo', 60, 100, 250);
GO