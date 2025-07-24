-- Crear la base de datos
USE [master];
GO

IF DB_ID('JNDataBase') IS NOT NULL
    DROP DATABASE JNDataBase;
GO

CREATE DATABASE [JNDataBase];
GO

USE [JNDataBase];
GO

-- Crear tabla de roles
CREATE TABLE [dbo].[TRol](
	[IdRol] INT IDENTITY(1,1) NOT NULL,
	[NombreRol] VARCHAR(50) NOT NULL,
 CONSTRAINT [PK_TRol] PRIMARY KEY CLUSTERED ([IdRol] ASC)
);
GO

-- Crear tabla de usuarios
CREATE TABLE [dbo].[TUsuario](
	[IdUsuario] BIGINT IDENTITY(1,1) NOT NULL,
	[Nombre] VARCHAR(255) NOT NULL,
	[Correo] VARCHAR(100) NOT NULL,
	[Identificacion] VARCHAR(20) NOT NULL,
	[Contrasenna] VARCHAR(255) NOT NULL,
	[Estado] BIT NOT NULL,
	[IdRol] INT NOT NULL,
 CONSTRAINT [PK_TUsuario] PRIMARY KEY CLUSTERED ([IdUsuario] ASC)
);
GO

-- Agregar restricciones únicas
ALTER TABLE [dbo].[TUsuario] 
ADD CONSTRAINT [uk_Correo] UNIQUE NONCLUSTERED ([Correo]);
GO

ALTER TABLE [dbo].[TUsuario] 
ADD CONSTRAINT [uk_Identificacion] UNIQUE NONCLUSTERED ([Identificacion]);
GO

-- Crear la relación entre usuario y rol
ALTER TABLE [dbo].[TUsuario]  WITH CHECK ADD  
CONSTRAINT [FK_TUsuario_TRol] 
FOREIGN KEY([IdRol]) REFERENCES [dbo].[TRol] ([IdRol]);
GO

ALTER TABLE [dbo].[TUsuario] CHECK CONSTRAINT [FK_TUsuario_TRol];
GO

-- Insertar datos de roles
SET IDENTITY_INSERT [dbo].[TRol] ON;
INSERT INTO [dbo].[TRol] ([IdRol], [NombreRol]) VALUES (1, N'Usuario Regular');
INSERT INTO [dbo].[TRol] ([IdRol], [NombreRol]) VALUES (2, N'Usuario Administrador');
SET IDENTITY_INSERT [dbo].[TRol] OFF;
GO

-- Insertar un usuario de ejemplo
SET IDENTITY_INSERT [dbo].[TUsuario] ON;
INSERT INTO [dbo].[TUsuario] 
([IdUsuario], [Nombre], [Correo], [Identificacion], [Contrasenna], [Estado], [IdRol]) 
VALUES 
(1, N'SEBASTIAN HERNANDEZ JIMENEZ', N'shernandez30307@ufide.ac.cr', N'402530307', N'RkG2IjacNXUd+XdPI72zSg==', 1, 1);
SET IDENTITY_INSERT [dbo].[TUsuario] OFF;
GO

-- Procedimiento para validar inicio de sesión
CREATE PROCEDURE [dbo].[ValidarInicioSesion]
	@Correo VARCHAR(100),
	@Contrasenna VARCHAR(255)
AS
BEGIN
	SELECT	IdUsuario,
			Nombre,
			Correo,
			Identificacion,
			Estado,
			U.IdRol,
			R.NombreRol
	FROM	dbo.TUsuario U
	INNER JOIN dbo.TRol R ON U.IdRol = R.IdRol
	WHERE	Correo = @Correo
		AND Contrasenna = @Contrasenna
		AND Estado = 1;
END;
GO


-- Procedimiento para registrar nuevo usuario
CREATE PROCEDURE [dbo].[RegistrarUsuario]
	@Nombre VARCHAR(255),
	@Correo VARCHAR(100),
	@Identificacion VARCHAR(20),
	@Contrasenna VARCHAR(255)
AS
BEGIN
	-- Verificar si el correo ya existe
	IF EXISTS (SELECT 1 FROM dbo.TUsuario WHERE Correo = @Correo)
	BEGIN
		SELECT -1 AS Resultado, 'El correo ya está registrado' AS Mensaje
		RETURN
	END

	-- Verificar si la identificación ya existe
	IF EXISTS (SELECT 1 FROM dbo.TUsuario WHERE Identificacion = @Identificacion)
	BEGIN
		SELECT -2 AS Resultado, 'La identificación ya está registrada' AS Mensaje
		RETURN
	END

	-- Insertar el nuevo usuario con rol de Usuario Regular (IdRol = 1)
	INSERT INTO dbo.TUsuario (Nombre, Correo, Identificacion, Contrasenna, Estado, IdRol)
	VALUES (@Nombre, @Correo, @Identificacion, @Contrasenna, 1, 1)

	-- Devolver el ID del usuario creado
	SELECT SCOPE_IDENTITY() AS Resultado, 'Usuario registrado exitosamente' AS Mensaje
END
GO



-- Agregar campos adicionales a la tabla TUsuario
ALTER TABLE [dbo].[TUsuario] 
ADD [Telefono] VARCHAR(20) NULL,
    [Direccion] VARCHAR(500) NULL,
    [FechaNacimiento] DATE NULL,
    [FotoPath] VARCHAR(255) NULL,
    [FechaRegistro] DATETIME2 DEFAULT GETDATE(),
    [FechaActualizacion] DATETIME2 DEFAULT GETDATE();
GO

-- Procedimiento para obtener perfil completo del usuario
CREATE PROCEDURE [dbo].[ObtenerPerfilUsuario]
    @IdUsuario BIGINT
AS
BEGIN
    SELECT  IdUsuario,
            Nombre,
            Correo,
            Identificacion,
            Telefono,
            Direccion,
            FechaNacimiento,
            FotoPath,
            Estado,
            U.IdRol,
            R.NombreRol,
            FechaRegistro,
            FechaActualizacion
    FROM    dbo.TUsuario U
    INNER JOIN dbo.TRol R ON U.IdRol = R.IdRol
    WHERE   IdUsuario = @IdUsuario
        AND Estado = 1;
END;
GO

-- Procedimiento para actualizar perfil básico (datos editables)
CREATE PROCEDURE [dbo].[ActualizarPerfilBasico]
    @IdUsuario BIGINT,
    @Nombre VARCHAR(255),
    @Correo VARCHAR(100),
    @Identificacion VARCHAR(20)
AS
BEGIN
    -- Verificar si el correo ya existe en otro usuario
    IF EXISTS (SELECT 1 FROM dbo.TUsuario WHERE Correo = @Correo AND IdUsuario != @IdUsuario)
    BEGIN
        SELECT -1 AS Resultado, 'El correo ya está registrado por otro usuario' AS Mensaje
        RETURN
    END
    
    -- Verificar si la identificación ya existe en otro usuario
    IF EXISTS (SELECT 1 FROM dbo.TUsuario WHERE Identificacion = @Identificacion AND IdUsuario != @IdUsuario)
    BEGIN
        SELECT -2 AS Resultado, 'La identificación ya está registrada por otro usuario' AS Mensaje
        RETURN
    END
    
    -- Actualizar datos básicos
    UPDATE dbo.TUsuario 
    SET Nombre = @Nombre,
        Correo = @Correo,
        Identificacion = @Identificacion,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
    
    SELECT 1 AS Resultado, 'Perfil actualizado exitosamente' AS Mensaje
END;
GO

-- Procedimiento para actualizar información adicional del perfil
CREATE PROCEDURE [dbo].[ActualizarInformacionAdicional]
    @IdUsuario BIGINT,
    @Telefono VARCHAR(20) = NULL,
    @Direccion VARCHAR(500) = NULL,
    @FechaNacimiento DATE = NULL,
    @FotoPath VARCHAR(255) = NULL
AS
BEGIN
    UPDATE dbo.TUsuario 
    SET Telefono = ISNULL(@Telefono, Telefono),
        Direccion = ISNULL(@Direccion, Direccion),
        FechaNacimiento = ISNULL(@FechaNacimiento, FechaNacimiento),
        FotoPath = ISNULL(@FotoPath, FotoPath),
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
    
    SELECT 1 AS Resultado, 'Información adicional actualizada exitosamente' AS Mensaje
END;
GO

-- Procedimiento para cambiar contraseña
CREATE PROCEDURE [dbo].[CambiarContrasena]
    @IdUsuario BIGINT,
    @ContrasenaActual VARCHAR(255),
    @ContrasenaNueva VARCHAR(255)
AS
BEGIN
    -- Verificar contraseña actual
    IF NOT EXISTS (SELECT 1 FROM dbo.TUsuario WHERE IdUsuario = @IdUsuario AND Contrasenna = @ContrasenaActual)
    BEGIN
        SELECT -1 AS Resultado, 'La contraseña actual es incorrecta' AS Mensaje
        RETURN
    END
    
    -- Actualizar contraseña
    UPDATE dbo.TUsuario 
    SET Contrasenna = @ContrasenaNueva,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
    
    SELECT 1 AS Resultado, 'Contraseña actualizada exitosamente' AS Mensaje
END;
GO