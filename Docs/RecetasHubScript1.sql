CREATE DATABASE RecetasHub;
GO
USE RecetasHub;
GO


-- 1. Tabla de Roles
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);
GO


-- 2. Tabla de Usuarios 
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id)
);
GO


/* 3. Tabla de Fuentes/Sources, es el "Catalogo" de las APIs o paginas donde vamos a leer la informacion de las recetas. Cada fuente puede ser un widget, una API, etc. 
    y se pueden agregar nuevas fuentes sin necesidad de cambiar la estructura de la base de datos, solo insertando nuevos registros en esta tabla.*/
CREATE TABLE Sources (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Url NVARCHAR(500) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ComponentType NVARCHAR(100) NOT NULL, -- e.g. 'widget', 'api', 'feed'
    RequiresSecret BIT NOT NULL DEFAULT 0 -- 0=no, 1=yes
);

GO
/* 4. Tabla de Items de Fuentes/SourceItems, es la "Bodega" de las recetas guardadas en el sistema. Donde los usuarios guardan las recetas de favoritos de acá la agarra y 
registra en UserFavorites, es la base del Import/Export de JSONs que pidió el profe. */
CREATE TABLE SourceItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceId INT FOREIGN KEY REFERENCES Sources(Id),
    JsonData NVARCHAR(MAX), -- Guardar el dato crudo de la API para poder exportarlo/importarlo
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

/* 5. Tabla de Configuraciones/Secrets, funciona para guardar claves de API u otras configuraciones sensibles que las fuentes 
      puedan necesitar. El uso sería guardar las Keys  de las APIs de ser necesario (si Tabla Sources, RequiresSecret = 1). */
CREATE TABLE SecretsSettings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KeyName NVARCHAR(100) NOT NULL UNIQUE,
    KeyValue NVARCHAR(500) NOT NULL,
    Description NVARCHAR(255) NULL
);
GO
/* 6. Tabla de Favoritos de Usuarios/UserFavorites, permite a los usuarios guardar sus recetas favoritas para acceder a ellas fácilmente en el futuro.
      Para hacer un "Top Favoritos" como pidió el profe podemos agrupar los resultados por SourceItemId y contarlos (COUNT) las que tengan conteo más alto son el top */
CREATE TABLE UserFavorites (
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    SourceItemId INT FOREIGN KEY REFERENCES SourceItems(Id),
    SavedAt DATETIME DEFAULT GETDATE(), 
    PRIMARY KEY (UserId, SourceItemId) -- Evita que un usuario guarde la misma receta dos veces
);
GO