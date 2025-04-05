-- Crear la base de datos
CREATE DATABASE RescateEmocional;
GO

-- Usar la base de datos
USE RescateEmocional;
GO

-- Creación de la tabla de Roles
CREATE TABLE Rol (
    IDRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255) NOT NULL
);
GO

-- Habilitar la inserción manual de IDs
SET IDENTITY_INSERT Rol ON;

-- Insertar roles en la tabla Rol
INSERT INTO Rol (IDRol, Nombre, Descripcion) 
VALUES 
    (1, 'Administrador', 'Son los administradores'),
    (2, 'Organizaciones', 'Son las organizaciones'),
    (3, 'Usuarios', 'Son los usuarios');

-- Deshabilitar la inserción manual de IDs
SET IDENTITY_INSERT Rol OFF;
GO

-- Creación de la tabla de Usuarios
CREATE TABLE Usuario (
    IDUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    CorreoElectronico NVARCHAR(100) UNIQUE NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    Estado TINYINT NOT NULL DEFAULT 1,   -- 1 = Activo, 0 = Inactivo
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Creación de la tabla de Organizaciones
CREATE TABLE Organizacion (
    IDOrganizacion INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    CorreoElectronico NVARCHAR(100) UNIQUE NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(255) NOT NULL,
    Horario NVARCHAR(50) NOT NULL,
    Ubicacion NVARCHAR(255) NOT NULL,
    Estado TINYINT NOT NULL DEFAULT 0,   -- 1 = Verificado, 0 = No verificado
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Creación de la tabla de Teléfonos
CREATE TABLE Telefono (
    IDTelefono INT IDENTITY(1,1) PRIMARY KEY,
    TipoDeNumero NVARCHAR(20) NOT NULL
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

-- Creación de la tabla de Etiquetas
CREATE TABLE Etiqueta (
    IDEtiqueta INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
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

-- Creación de la tabla de Administradores
CREATE TABLE Administrador (
    IDAdmin INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    CorreoElectronico NVARCHAR(100) UNIQUE NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    IDRol INT NOT NULL,
    FOREIGN KEY (IDRol) REFERENCES Rol(IDRol)
);
GO

-- Creación de la tabla de Peticiones de Verificación
CREATE TABLE PeticionVerificacion (
    IDPeticion INT IDENTITY(1,1) PRIMARY KEY,
    IDOrganizacion INT NOT NULL,
    Estado TINYINT NOT NULL DEFAULT 0,  -- 0 = Pendiente, 1 = Aprobado, 2 = Rechazado
    FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE(),
    Comentarios NVARCHAR(255) NULL,
    IDAdmin INT NOT NULL,
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion),
    FOREIGN KEY (IDAdmin) REFERENCES Administrador(IDAdmin)
);
GO

-- Creación de la tabla de Conversaciones
CREATE TABLE Conversacion (
    IDConversacion INT IDENTITY(1,1) PRIMARY KEY,
    IDUsuario INT NOT NULL,
    IDOrganizacion INT NOT NULL,
    FechaInicio DATETIME NOT NULL DEFAULT GETDATE(),
    Mensaje NVARCHAR(MAX) NULL,
    Emisor NVARCHAR(50) NULL,
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion)
);
GO

-- Creación de la tabla de Mensajes
CREATE TABLE Mensaje (
    IDMensaje INT IDENTITY(1,1) PRIMARY KEY,
    IDConversacion INT NOT NULL,
    IDUsuario INT NULL,
    IDOrganizacion INT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    Estado TINYINT NOT NULL DEFAULT 0,  -- 0 = Enviado, 1 = Leído, 2 = Respondido
    FOREIGN KEY (IDConversacion) REFERENCES Conversacion(IDConversacion),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario),
    FOREIGN KEY (IDOrganizacion) REFERENCES Organizacion(IDOrganizacion)
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

-- Creación de la tabla de Diarios
CREATE TABLE Diario (
    IDDiario INT IDENTITY(1,1) PRIMARY KEY,
    IDUsuario INT NOT NULL,
    Titulo NVARCHAR(100) NOT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);
GO

-- Insertar datos de prueba
INSERT INTO Usuario (Nombre, CorreoElectronico, Telefono, Contrasena, Estado, IDRol) 
VALUES 
    ('Usuario1', 'usuario1@email.com', '123-456-7890', 'e10adc3949ba59abbe56e057f20f883e', 1, 3),
    ('Usuario2', 'usuario2@email.com', '098-765-4321', 'e10adc3949ba59abbe56e057f20f883e', 1, 3);
GO

INSERT INTO Organizacion (Nombre, CorreoElectronico, Contrasena, Descripcion, Horario, Ubicacion, Estado, IDRol) 
VALUES 
    ('Org1', 'org1@email.com', 'e10adc3949ba59abbe56e057f20f883e', 'Organización de apoyo emocional', '9:00-17:00', 'Ciudad X', 0, 2),
    ('Org2', 'org2@email.com', 'e10adc3949ba59abbe56e057f20f883e', 'Ayuda psicológica', '10:00-18:00', 'Ciudad Y', 0, 2);
GO

INSERT INTO Administrador (Nombre, CorreoElectronico, Contrasena, IDRol)
VALUES 
    ('Admin1', 'admin1@email.com', 'e10adc3949ba59abbe56e057f20f883e', 1);
GO

INSERT INTO Conversacion (IDUsuario, IDOrganizacion, Mensaje, Emisor)
VALUES 
    (1, 1, 'Hola, necesito ayuda', 'Usuario'),
    (2, 2, 'Buenas, ¿cómo podemos ayudar?', 'Organizacion');
GO

INSERT INTO Mensaje (IDConversacion, IDUsuario, IDOrganizacion, Contenido, FechaHora, Estado)
VALUES 
    (1, 1, NULL, 'Hola, necesito ayuda', GETDATE(), 0),
    (2, NULL, 2, 'Buenas, ¿cómo podemos ayudar?', GETDATE(), 0);
GO