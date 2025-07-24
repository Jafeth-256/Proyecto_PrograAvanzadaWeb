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





-- Procedimiento para obtener todos los usuarios
CREATE PROCEDURE [dbo].[ObtenerTodosLosUsuarios]
AS
BEGIN
    SELECT  U.IdUsuario,
            U.Nombre,
            U.Correo,
            U.Identificacion,
            U.Estado,
            U.IdRol,
            R.NombreRol,
            U.Telefono,
            U.Direccion,
            U.FechaNacimiento,
            U.FotoPath,
            U.FechaRegistro,
            U.FechaActualizacion
    FROM    dbo.TUsuario U
    INNER JOIN dbo.TRol R ON U.IdRol = R.IdRol
    ORDER BY U.FechaRegistro DESC;
END;
GO

-- Procedimiento para obtener un usuario por ID
CREATE PROCEDURE [dbo].[ObtenerUsuarioPorId]
    @IdUsuario BIGINT
AS
BEGIN
    SELECT  U.IdUsuario,
            U.Nombre,
            U.Correo,
            U.Identificacion,
            U.Estado,
            U.IdRol,
            R.NombreRol,
            U.Telefono,
            U.Direccion,
            U.FechaNacimiento,
            U.FotoPath,
            U.FechaRegistro,
            U.FechaActualizacion
    FROM    dbo.TUsuario U
    INNER JOIN dbo.TRol R ON U.IdRol = R.IdRol
    WHERE   U.IdUsuario = @IdUsuario;
END;
GO

-- Procedimiento para actualizar usuario (incluyendo estado y rol)
CREATE PROCEDURE [dbo].[ActualizarUsuario]
    @IdUsuario BIGINT,
    @Nombre VARCHAR(255),
    @Correo VARCHAR(100),
    @Identificacion VARCHAR(20),
    @Estado BIT,
    @IdRol INT
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
    
    -- Verificar si el rol existe
    IF NOT EXISTS (SELECT 1 FROM dbo.TRol WHERE IdRol = @IdRol)
    BEGIN
        SELECT -3 AS Resultado, 'El rol seleccionado no existe' AS Mensaje
        RETURN
    END
    
    -- Actualizar el usuario
    UPDATE dbo.TUsuario 
    SET Nombre = @Nombre,
        Correo = @Correo,
        Identificacion = @Identificacion,
        Estado = @Estado,
        IdRol = @IdRol,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
    
    SELECT 1 AS Resultado, 'Usuario actualizado exitosamente' AS Mensaje
END;
GO

-- Procedimiento para cambiar solo el estado del usuario
CREATE PROCEDURE [dbo].[CambiarEstadoUsuario]
    @IdUsuario BIGINT,
    @Estado BIT
AS
BEGIN
    -- Verificar si el usuario existe
    IF NOT EXISTS (SELECT 1 FROM dbo.TUsuario WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT -1 AS Resultado, 'El usuario no existe' AS Mensaje
        RETURN
    END
    
    -- Actualizar solo el estado
    UPDATE dbo.TUsuario 
    SET Estado = @Estado,
        FechaActualizacion = GETDATE()
    WHERE IdUsuario = @IdUsuario;
    
    DECLARE @Accion VARCHAR(20) = CASE WHEN @Estado = 1 THEN 'activado' ELSE 'desactivado' END;
    
    SELECT 1 AS Resultado, 'Usuario ' + @Accion + ' exitosamente' AS Mensaje
END;
GO

-- Procedimiento para obtener estadísticas de usuarios
CREATE PROCEDURE [dbo].[ObtenerEstadisticasUsuarios]
AS
BEGIN
    SELECT 
        COUNT(*) as TotalUsuarios,
        SUM(CASE WHEN Estado = 1 THEN 1 ELSE 0 END) as UsuariosActivos,
        SUM(CASE WHEN Estado = 0 THEN 1 ELSE 0 END) as UsuariosInactivos,
        SUM(CASE WHEN IdRol = 1 THEN 1 ELSE 0 END) as UsuariosRegulares,
        SUM(CASE WHEN IdRol = 2 THEN 1 ELSE 0 END) as UsuariosAdministradores
    FROM dbo.TUsuario;
END;
GO


-- Agregar tabla de Tours
CREATE TABLE [dbo].[TTour](
	[IdTour] BIGINT IDENTITY(1,1) NOT NULL,
	[Nombre] VARCHAR(255) NOT NULL,
	[Descripcion] TEXT NOT NULL,
	[Destino] VARCHAR(255) NOT NULL,
	[Precio] DECIMAL(10,2) NOT NULL,
	[FechaInicio] DATE NOT NULL,
	[FechaFin] DATE NOT NULL,
	[CantidadPersonas] INT NOT NULL,
	[Estado] BIT NOT NULL DEFAULT 1,
	[FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
	[IdUsuarioCreador] BIGINT NOT NULL,
 CONSTRAINT [PK_TTour] PRIMARY KEY CLUSTERED ([IdTour] ASC)
);
GO

-- Crear la relación con usuario creador
ALTER TABLE [dbo].[TTour] WITH CHECK ADD  
CONSTRAINT [FK_TTour_TUsuario] 
FOREIGN KEY([IdUsuarioCreador]) REFERENCES [dbo].[TUsuario] ([IdUsuario]);
GO
ALTER TABLE [dbo].[TTour] CHECK CONSTRAINT [FK_TTour_TUsuario];
GO

-- Procedimiento para consultar todos los tours
CREATE PROCEDURE [dbo].[ConsultarTours]
AS
BEGIN
	SELECT	T.IdTour,
			T.Nombre,
			T.Descripcion,
			T.Destino,
			T.Precio,
			T.FechaInicio,
			T.FechaFin,
			T.CantidadPersonas,
			T.Estado,
			T.FechaCreacion,
			T.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TTour T
	INNER JOIN dbo.TUsuario U ON T.IdUsuarioCreador = U.IdUsuario
	WHERE	T.Estado = 1
	ORDER BY T.FechaCreacion DESC;
END;
GO

-- Procedimiento para consultar un tour por ID
CREATE PROCEDURE [dbo].[ConsultarTourPorId]
	@IdTour BIGINT
AS
BEGIN
	SELECT	T.IdTour,
			T.Nombre,
			T.Descripcion,
			T.Destino,
			T.Precio,
			T.FechaInicio,
			T.FechaFin,
			T.CantidadPersonas,
			T.Estado,
			T.FechaCreacion,
			T.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TTour T
	INNER JOIN dbo.TUsuario U ON T.IdUsuarioCreador = U.IdUsuario
	WHERE	T.IdTour = @IdTour
		AND T.Estado = 1;
END;
GO

-- Procedimiento para registrar nuevo tour
CREATE PROCEDURE [dbo].[RegistrarTour]
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Destino VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATE,
	@FechaFin DATE,
	@CantidadPersonas INT,
	@IdUsuarioCreador BIGINT
AS
BEGIN
	-- Validar que la fecha de fin sea mayor que la fecha de inicio
	IF @FechaFin <= @FechaInicio
	BEGIN
		SELECT -1 AS Resultado, 'La fecha de fin debe ser posterior a la fecha de inicio' AS Mensaje
		RETURN
	END

	-- Validar que las fechas sean futuras
	IF @FechaInicio <= GETDATE()
	BEGIN
		SELECT -2 AS Resultado, 'La fecha de inicio debe ser futura' AS Mensaje
		RETURN
	END

	-- Insertar el nuevo tour
	INSERT INTO dbo.TTour (Nombre, Descripcion, Destino, Precio, FechaInicio, FechaFin, CantidadPersonas, IdUsuarioCreador, Estado)
	VALUES (@Nombre, @Descripcion, @Destino, @Precio, @FechaInicio, @FechaFin, @CantidadPersonas, @IdUsuarioCreador, 1)

	-- Devolver el ID del tour creado
	SELECT SCOPE_IDENTITY() AS Resultado, 'Tour registrado exitosamente' AS Mensaje
END
GO

-- Procedimiento para actualizar tour
CREATE PROCEDURE [dbo].[ActualizarTour]
	@IdTour BIGINT,
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Destino VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATE,
	@FechaFin DATE,
	@CantidadPersonas INT
AS
BEGIN
	-- Validar que el tour existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TTour WHERE IdTour = @IdTour AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El tour no existe o está inactivo' AS Mensaje
		RETURN
	END

	-- Validar que la fecha de fin sea mayor que la fecha de inicio
	IF @FechaFin <= @FechaInicio
	BEGIN
		SELECT -2 AS Resultado, 'La fecha de fin debe ser posterior a la fecha de inicio' AS Mensaje
		RETURN
	END

	-- Actualizar el tour
	UPDATE dbo.TTour 
	SET Nombre = @Nombre,
		Descripcion = @Descripcion,
		Destino = @Destino,
		Precio = @Precio,
		FechaInicio = @FechaInicio,
		FechaFin = @FechaFin,
		CantidadPersonas = @CantidadPersonas
	WHERE IdTour = @IdTour

	SELECT 1 AS Resultado, 'Tour actualizado exitosamente' AS Mensaje
END
GO

-- Procedimiento para eliminar tour (eliminación lógica)
CREATE PROCEDURE [dbo].[EliminarTour]
	@IdTour BIGINT
AS
BEGIN
	-- Validar que el tour existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TTour WHERE IdTour = @IdTour AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El tour no existe o ya está eliminado' AS Mensaje
		RETURN
	END

	-- Eliminar lógicamente el tour
	UPDATE dbo.TTour 
	SET Estado = 0
	WHERE IdTour = @IdTour

	SELECT 1 AS Resultado, 'Tour eliminado exitosamente' AS Mensaje
END
GO


