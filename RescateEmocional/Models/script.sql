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
    CorreoElectronico VARCHAR(100) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL,
    Descripcion TEXT NOT NULL,
    Horario VARCHAR(50) NOT NULL,
    Ubicacion VARCHAR(255) NOT NULL,
    Estado TINYINT NOT NULL,  -- 1 = Verificado, 0 = No verificado
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Crear la tabla de tel�fonos
CREATE TABLE Telefono (
    IDTelefono INT IDENTITY(1,1) PRIMARY KEY,
    TipoDeNumero VARCHAR(20) NOT NULL
);
GO

-- Tabla intermedia: Relaci�n Organizacion - Telefono
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

-- Tabla intermedia: Relaci�n Organizacion - Etiqueta
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

-- Crear la tabla de peticiones de verificaci�n
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
    Emisor NVARCHAR(50) NULL,
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
    IDUsuario INT NULL,
    IDOrganizacion INT NULL,
    Estado TINYINT NOT NULL,  -- 0 = Enviado, 1 = Le�do, 2 = Respondido
    FOREIGN KEY (IDConversacion) REFERENCES Conversacion(IDConversacion),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion)
);
GO

-- Tabla intermedia: Relaci�n Administrador - Usuario
CREATE TABLE AdministradorUsuario (
    IDAdmin INT,
    IDUsuario INT,
    PRIMARY KEY (IDAdmin, IDUsuario),
    FOREIGN KEY (IDAdmin) REFERENCES Administrador(IDAdmin),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);
GO

-- Tabla intermedia: Relaci�n Administrador - Organizacion
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

SET IDENTITY_INSERT Rol ON;

INSERT INTO Rol (IDRol, Nombre, Descripcion) 
VALUES 
    (1, 'Administrador', 'Son los administradores'),
    (2, 'Organizaciones', 'Son las organizaciones'),
    (3, 'Usuarios', 'Son los usuarios');

SET IDENTITY_INSERT Rol OFF;

INSERT INTO Administrador (Nombre, CorreoElectronico, Contrasena, IDRol)
VALUES ('Admin Principal', 'admin@email.com', '5c2e5fb3697ee858793d25c3efda62d6', 1);

-- Insertar una Organizaci�n
INSERT INTO Organizacion (Nombre, Descripcion, Horario, Ubicacion, Estado, IDRol, CorreoElectronico, Contrasena) 
VALUES ('Ayuda Social', 'Organizaci�n dedicada al bienestar emocional.', 'Lunes a Viernes, 9AM - 6PM', 'Calle Falsa 123, Ciudad', 1, 2, 'Organizacion@gmail.com', '5c2e5fb3697ee858793d25c3efda62d6');

-- Insertar un Usuario
INSERT INTO Usuario (Nombre, CorreoElectronico, Telefono, Contrasena, Estado, IDRol)
VALUES ('Juan P�rez', 'usuario@email.com', '123456789', '5c2e5fb3697ee858793d25c3efda62d6', 1, 3);

-- contrase�a Hola503@
