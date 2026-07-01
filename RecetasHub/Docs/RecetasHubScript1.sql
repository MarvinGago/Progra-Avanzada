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









USE RecetasHub;
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Administrador')
    INSERT INTO Roles (RoleName) VALUES ('Administrador');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Estándar')
    INSERT INTO Roles (RoleName) VALUES ('Estándar');
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin_recetashub')
    INSERT INTO Users (Username, PasswordHash, RoleId)
    VALUES (
        'admin_recetashub',
        'PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=',
        1
    );
GO





CREATE or ALTER PROCEDURE sp_RegisterUser
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @DefaultRoleId INT = 2 -- 2 = Estándar, 1 = Admin
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar si el usuario ya existe
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT -1 AS ResultCode, 'El nombre de usuario ya está en uso.' AS Message;
        RETURN;
    END

    -- Insertar el nuevo usuario
    INSERT INTO Users (Username, PasswordHash, RoleId)
    VALUES (@Username, @PasswordHash, @DefaultRoleId);

    SELECT 1 AS ResultCode, 'Usuario registrado exitosamente.' AS Message;
END;
GO






CREATE or ALTER PROCEDURE sp_LoginUser
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id, 
        u.Username, 
        u.PasswordHash, 
        u.RoleId,
        r.RoleName
    FROM Users u
    INNER JOIN Roles r ON u.RoleId = r.Id
    WHERE u.Username = @Username;
END;
GO









USE RecetasHub;
GO

CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        u.Id,
        u.Username,
        u.RoleId,
        r.RoleName
    FROM Users u
    INNER JOIN Roles r ON u.RoleId = r.Id
    ORDER BY u.Id;
END
GO

CREATE OR ALTER PROCEDURE sp_ChangeUserRole
    @UserId INT,
    @NewRoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId)
    BEGIN
        SELECT -1 AS ResultCode, 'Usuario no encontrado.' AS Message;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @NewRoleId)
    BEGIN
        SELECT -1 AS ResultCode, 'Rol no válido.' AS Message;
        RETURN;
    END

    UPDATE Users SET RoleId = @NewRoleId WHERE Id = @UserId;
    SELECT 1 AS ResultCode, 'Rol actualizado correctamente.' AS Message;
END
GO






CREATE OR ALTER PROCEDURE sp_DeleteUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @UserId)
    BEGIN
        SELECT -1 AS ResultCode, 'Usuario no encontrado.' AS Message;
        RETURN;
    END

    -- Primero eliminamos sus favoritos para no violar FK
    DELETE FROM UserFavorites WHERE UserId = @UserId;
    DELETE FROM Users WHERE Id = @UserId;

    SELECT 1 AS ResultCode, 'Usuario eliminado correctamente.' AS Message;
END
GO