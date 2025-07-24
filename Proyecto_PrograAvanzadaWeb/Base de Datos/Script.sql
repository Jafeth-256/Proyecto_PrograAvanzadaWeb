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
