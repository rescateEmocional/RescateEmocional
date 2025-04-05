 -- Crear la base de datos
CREATE DATABASE RescateEmocional;
GO

-- Usar la base de datos
USE RescateEmocional;
GO

-- Crear la tabla de roles
CREATE TABLE Rol (
    IDRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion TEXT
);
GO

-- Crear la tabla de usuarios
CREATE TABLE Usuario (
    IDUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    Telefono VARCHAR(20) NOT NULL,
    Contrasena VARCHAR(255) NOT NULL,
    Estado TINYINT NOT NULL,  -- 1 = Activo, 0 = Inactivo
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Crear la tabla de organizaciones
CREATE TABLE Organizacion (
    IDOrganizacion INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion TEXT NOT NULL,
    Horario VARCHAR(50) NOT NULL,
    Ubicacion VARCHAR(255) NOT NULL,
    Estado TINYINT NOT NULL,  -- 1 = Verificado, 0 = No verificado
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Crear la tabla de teléfonos
CREATE TABLE Telefono (
    IDTelefono INT IDENTITY(1,1) PRIMARY KEY,
    TipoDeNumero VARCHAR(20) NOT NULL
);
GO

-- Tabla intermedia: Relación Organizacion - Telefono
CREATE TABLE OrganizacionTelefono (
    IDOrganizacion INT,
    IDTelefono INT,
    PRIMARY KEY (IDOrganizacion, IDTelefono),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion),
    FOREIGN KEY (IDTelefono) REFERENCES Telefono(IDTelefono)
);
GO

-- Crear la tabla de etiquetas
CREATE TABLE Etiqueta (
    IDEtiqueta INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);
GO

-- Tabla intermedia: Relación Organizacion - Etiqueta
CREATE TABLE OrganizacionEtiqueta (
    IDOrganizacion INT,
    IDEtiqueta INT,
    PRIMARY KEY (IDOrganizacion, IDEtiqueta),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion),
    FOREIGN KEY (IDEtiqueta) REFERENCES Etiqueta(IDEtiqueta)
);
GO

-- Crear la tabla de administradores
CREATE TABLE Administrador (
    IDAdmin INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL,
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Crear la tabla de peticiones de verificación
CREATE TABLE PeticionVerificacion (
    IDPeticion INT IDENTITY(1,1) PRIMARY KEY,
    IDOrganizacion INT NOT NULL,
    Estado TINYINT NOT NULL,  -- 0 = Pendiente, 1 = Aprobado, 2 = Rechazado
    FechaSolicitud DATETIME NOT NULL,
    Comentarios TEXT NULL,
    IDAdmin INT NOT NULL,
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion),
    FOREIGN KEY (IDAdmin) REFERENCES Administrador(IDAdmin)
);
GO

-- Crear la tabla de conversaciones
CREATE TABLE Conversacion (
    IDConversacion INT IDENTITY(1,1) PRIMARY KEY,
    IDUsuario INT NOT NULL,
    IDOrganizacion INT NOT NULL,
    FechaInicio DATETIME NOT NULL,
    Mensaje TEXT NULL,
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion)
);
GO

-- Crear la tabla de mensajes
CREATE TABLE Mensaje (
    IDMensaje INT IDENTITY(1,1) PRIMARY KEY,
    IDConversacion INT NOT NULL,
    Contenido TEXT NOT NULL,
    FechaHora DATETIME NOT NULL,
    Estado TINYINT NOT NULL,  -- 0 = Enviado, 1 = Leído, 2 = Respondido
    FOREIGN KEY (IDConversacion) REFERENCES Conversacion(IDConversacion)
);
GO

-- Tabla intermedia: Relación Administrador - Usuario
CREATE TABLE AdministradorUsuario (
    IDAdmin INT,
    IDUsuario INT,
    PRIMARY KEY (IDAdmin, IDUsuario),
    FOREIGN KEY (IDAdmin) REFERENCES Administrador(IDAdmin),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);
GO

-- Tabla intermedia: Relación Administrador - Organizacion
CREATE TABLE AdministradorOrganizacion (
    IDAdmin INT,
    IDOrganizacion INT,
    PRIMARY KEY (IDAdmin, IDOrganizacion),
    FOREIGN KEY (IDAdmin) REFERENCES Administrador(IDAdmin),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion)
);
GO

-- Crear la tabla de diarios
CREATE TABLE Diario (
    IDDiario INT IDENTITY(1,1) PRIMARY KEY,
    IDUsuario INT NOT NULL,
    Titulo VARCHAR(100) NOT NULL,
    Contenido TEXT NOT NULL,
    FechaCreacion DATETIME NOT NULL,
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);
GO



use RescateEmocional;
-- Insertar un Administrador
INSERT INTO Administrador (Nombre, CorreoElectronico, Contrasena, IDRol)
VALUES ('Admin Principal', 'admin@email.com', '123456', 1);

-- Insertar una Organización
INSERT INTO Organizacion (Nombre, Descripcion, Horario, Ubicacion, Estado, IDRol, CorreoElectronico, Contrasena) 
VALUES ('Ayuda Social', 'Organización dedicada al bienestar emocional.', 'Lunes a Viernes, 9AM - 6PM', 'Calle Falsa 123, Ciudad', 1, 2, 'Organizacion@gmail.com', '123456');

-- Insertar un Usuario
INSERT INTO Usuario (Nombre, CorreoElectronico, Telefono, Contrasena, Estado, IDRol)
VALUES ('Juan Pérez', 'usuario@email.com', '123456789', '123456', 1, 3);

SET IDENTITY_INSERT Rol ON;

INSERT INTO Rol (IDRol, Nombre, Descripcion) 
VALUES 
    (1, 'Administrador', 'Son los administradores'),
    (2, 'Organizaciones', 'Son las organizaciones'),
    (3, 'Usuarios', 'Son los usuarios');

SET IDENTITY_INSERT Rol OFF;


SELECT * FROM Administrador;
SELECT * FROM Organizacion;
SELECT * FROM Usuario;
SELECT * FROM Rol;


DELETE FROM Usuario;
DELETE FROM Administrador;
DELETE FROM Organizacion;

ALTER TABLE Organizacion 
ADD CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL;


  

    USE RescateEmocional;
GO

-- Rol
ALTER TABLE Rol
ALTER COLUMN Descripcion NVARCHAR(255);
GO

-- Usuario
ALTER TABLE Usuario
ALTER COLUMN Telefono VARCHAR(20) NULL;
GO

ALTER TABLE Usuario
ADD CONSTRAINT DF_Usuario_Estado DEFAULT 1 FOR Estado;
GO

-- Organizacion
ALTER TABLE Organizacion
ADD CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL;
GO

ALTER TABLE Organizacion
ADD CONSTRAINT DF_Organizacion_Estado DEFAULT 0 FOR Estado;
GO

-- PeticionVerificacion
ALTER TABLE PeticionVerificacion
ADD CONSTRAINT DF_PeticionVerificacion_Estado DEFAULT 0 FOR Estado;
GO

ALTER TABLE PeticionVerificacion
ADD CONSTRAINT DF_PeticionVerificacion_FechaSolicitud DEFAULT GETDATE() FOR FechaSolicitud;
GO

-- Conversacion
ALTER TABLE Conversacion
ADD Emisor NVARCHAR(50) NULL;
GO

ALTER TABLE Conversacion
ADD CONSTRAINT DF_Conversacion_FechaInicio DEFAULT GETDATE() FOR FechaInicio;
GO

-- Mensaje
ALTER TABLE Mensaje
ADD IDUsuario INT NULL,
    IDOrganizacion INT NULL;
GO

ALTER TABLE Mensaje
ADD CONSTRAINT FK_Mensaje_Usuario FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario),
    CONSTRAINT FK_Mensaje_Organizacion FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion);
GO

ALTER TABLE Mensaje
ADD CONSTRAINT DF_Mensaje_FechaHora DEFAULT GETDATE() FOR FechaHora,
    CONSTRAINT DF_Mensaje_Estado DEFAULT 0 FOR Estado;
GO