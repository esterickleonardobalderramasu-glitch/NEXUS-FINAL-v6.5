   IF DB_ID(N'DeadSignal_DB') IS NULL
BEGIN
    CREATE DATABASE DeadSignal_DB;
END
GO

USE DeadSignal_DB;
GO

IF OBJECT_ID('dbo.BitacoraEventos', 'U') IS NOT NULL DROP TABLE dbo.BitacoraEventos;
IF OBJECT_ID('dbo.EntidadesMapa', 'U') IS NOT NULL DROP TABLE dbo.EntidadesMapa;
IF OBJECT_ID('dbo.EstadoCadete', 'U') IS NOT NULL DROP TABLE dbo.EstadoCadete;
IF OBJECT_ID('dbo.Partidas', 'U') IS NOT NULL DROP TABLE dbo.Partidas;
IF OBJECT_ID('dbo.Jugadores', 'U') IS NOT NULL DROP TABLE dbo.Jugadores;
GO

CREATE TABLE Jugadores (
    IdJugador INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    CodigoCadete NVARCHAR(30) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Partidas (
    IdPartida INT IDENTITY(1,1) PRIMARY KEY,
    IdJugador INT NOT NULL,
    NombreRanura NVARCHAR(80) NOT NULL,
    SectorActual NVARCHAR(50) NOT NULL DEFAULT 'Cubierta 15x12',
    TurnosJugados INT NOT NULL DEFAULT 0,
    EstadoPartida NVARCHAR(20) NOT NULL DEFAULT 'Activa',
    UltimaActualizacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Partidas_Jugadores FOREIGN KEY (IdJugador)
        REFERENCES Jugadores(IdJugador) ON DELETE CASCADE
);

CREATE TABLE EstadoCadete (
    IdEstado INT IDENTITY(1,1) PRIMARY KEY,
    IdPartida INT NOT NULL UNIQUE,
    VidaActual INT NOT NULL DEFAULT 100,
    VidaMax INT NOT NULL DEFAULT 100,
    ExperienciaXP INT NOT NULL DEFAULT 0,
    Nivel INT NOT NULL DEFAULT 1,
    PosicionX INT NOT NULL DEFAULT 0,
    PosicionY INT NOT NULL DEFAULT 0,
    CargasEMP INT NOT NULL DEFAULT 2,
    CONSTRAINT FK_EstadoCadete_Partidas FOREIGN KEY (IdPartida)
        REFERENCES Partidas(IdPartida) ON DELETE CASCADE
);

CREATE TABLE EntidadesMapa (
    IdEntidad INT IDENTITY(1,1) PRIMARY KEY,
    IdPartida INT NOT NULL,
    TipoEntidad NVARCHAR(40) NOT NULL,
    PosX INT NOT NULL,
    PosY INT NOT NULL,
    Vida INT NOT NULL DEFAULT 1,
    Estado NVARCHAR(30) NOT NULL DEFAULT 'Activo',
    CONSTRAINT FK_EntidadesMapa_Partidas FOREIGN KEY (IdPartida)
        REFERENCES Partidas(IdPartida) ON DELETE CASCADE
);

CREATE TABLE BitacoraEventos (
    IdEvento INT IDENTITY(1,1) PRIMARY KEY,
    IdPartida INT NOT NULL,
    FechaEvento DATETIME NOT NULL DEFAULT GETDATE(),
    TipoAccion NVARCHAR(30) NOT NULL,
    Detalle NVARCHAR(250) NOT NULL,
    CONSTRAINT FK_BitacoraEventos_Partidas FOREIGN KEY (IdPartida)
        REFERENCES Partidas(IdPartida) ON DELETE CASCADE
);
GO
