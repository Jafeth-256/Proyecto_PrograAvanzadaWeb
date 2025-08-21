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


-- Crear tabla de reservas
CREATE TABLE [dbo].[TReserva](
	[IdReserva] BIGINT IDENTITY(1,1) NOT NULL,
	[IdTour] BIGINT NOT NULL,
	[IdUsuario] BIGINT NOT NULL,
	[CantidadPersonas] INT NOT NULL,
	[PrecioTotal] DECIMAL(10,2) NOT NULL,
	[FechaReserva] DATETIME NOT NULL DEFAULT GETDATE(),
	[EstadoReserva] VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
	[Comentarios] VARCHAR(500) NULL,
	[FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
	[FechaActualizacion] DATETIME NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_TReserva] PRIMARY KEY CLUSTERED ([IdReserva] ASC)
);
GO

-- Crear relaciones
ALTER TABLE [dbo].[TReserva]  WITH CHECK ADD  
CONSTRAINT [FK_TReserva_TTour] 
FOREIGN KEY([IdTour]) REFERENCES [dbo].[TTour] ([IdTour]);
GO

ALTER TABLE [dbo].[TReserva]  WITH CHECK ADD  
CONSTRAINT [FK_TReserva_TUsuario] 
FOREIGN KEY([IdUsuario]) REFERENCES [dbo].[TUsuario] ([IdUsuario]);
GO

ALTER TABLE [dbo].[TReserva] CHECK CONSTRAINT [FK_TReserva_TTour];
GO
ALTER TABLE [dbo].[TReserva] CHECK CONSTRAINT [FK_TReserva_TUsuario];
GO

-- Procedimiento para consultar tours disponibles para reserva
CREATE PROCEDURE [dbo].[ConsultarToursDisponibles]
AS
BEGIN
	SELECT	T.IdTour,
			T.Nombre,
			CAST(T.Descripcion AS VARCHAR(MAX)) AS Descripcion,
			T.Destino,
			T.Precio,
			T.FechaInicio,
			T.FechaFin,
			T.CantidadPersonas,
			T.FechaCreacion,
			U.Nombre AS NombreCreador,
			ISNULL(SUM(R.CantidadPersonas), 0) AS PersonasReservadas,
			(T.CantidadPersonas - ISNULL(SUM(R.CantidadPersonas), 0)) AS CuposDisponibles
	FROM	dbo.TTour T
	INNER JOIN dbo.TUsuario U ON T.IdUsuarioCreador = U.IdUsuario
	LEFT JOIN dbo.TReserva R ON T.IdTour = R.IdTour AND R.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE	T.Estado = 1
		AND T.FechaInicio > GETDATE()
	GROUP BY T.IdTour, T.Nombre, CAST(T.Descripcion AS VARCHAR(MAX)), T.Destino, T.Precio, 
			 T.FechaInicio, T.FechaFin, T.CantidadPersonas, T.FechaCreacion, U.Nombre
	HAVING (T.CantidadPersonas - ISNULL(SUM(R.CantidadPersonas), 0)) > 0
	ORDER BY T.FechaInicio ASC;
END;
GO

-- Procedimiento para consultar un tour específico con disponibilidad
CREATE PROCEDURE [dbo].[ConsultarTourDisponiblePorId]
	@IdTour BIGINT
AS
BEGIN
	SELECT	T.IdTour,
			T.Nombre,
			CAST(T.Descripcion AS VARCHAR(MAX)) AS Descripcion,
			T.Destino,
			T.Precio,
			T.FechaInicio,
			T.FechaFin,
			T.CantidadPersonas,
			T.FechaCreacion,
			U.Nombre AS NombreCreador,
			ISNULL(SUM(R.CantidadPersonas), 0) AS PersonasReservadas,
			(T.CantidadPersonas - ISNULL(SUM(R.CantidadPersonas), 0)) AS CuposDisponibles
	FROM	dbo.TTour T
	INNER JOIN dbo.TUsuario U ON T.IdUsuarioCreador = U.IdUsuario
	LEFT JOIN dbo.TReserva R ON T.IdTour = R.IdTour AND R.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE	T.IdTour = @IdTour
		AND T.Estado = 1
		AND T.FechaInicio > GETDATE()
	GROUP BY T.IdTour, T.Nombre, CAST(T.Descripcion AS VARCHAR(MAX)), T.Destino, T.Precio, 
			 T.FechaInicio, T.FechaFin, T.CantidadPersonas, T.FechaCreacion, U.Nombre;
END;
GO

-- Procedimiento para crear una reserva
CREATE PROCEDURE [dbo].[CrearReserva]
	@IdTour BIGINT,
	@IdUsuario BIGINT,
	@CantidadPersonas INT,
	@Comentarios VARCHAR(500) = NULL
AS
BEGIN
	BEGIN TRANSACTION;
	
	DECLARE @PrecioUnitario DECIMAL(10,2);
	DECLARE @CuposDisponibles INT;
	DECLARE @TourValido BIT = 0;
	
	-- Verificar si el tour existe y está disponible
	SELECT @PrecioUnitario = T.Precio,
		   @CuposDisponibles = (T.CantidadPersonas - ISNULL(SUM(R.CantidadPersonas), 0)),
		   @TourValido = 1
	FROM dbo.TTour T
	LEFT JOIN dbo.TReserva R ON T.IdTour = R.IdTour AND R.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE T.IdTour = @IdTour 
		AND T.Estado = 1 
		AND T.FechaInicio > GETDATE()
	GROUP BY T.IdTour, T.Precio, T.CantidadPersonas;
	
	IF @TourValido = 0
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -1 AS Resultado, 'El tour no existe o no está disponible para reserva' AS Mensaje;
		RETURN;
	END
	
	-- Verificar disponibilidad de cupos
	IF @CuposDisponibles < @CantidadPersonas
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -2 AS Resultado, 'No hay suficientes cupos disponibles. Cupos disponibles: ' + CAST(@CuposDisponibles AS VARCHAR(10)) AS Mensaje;
		RETURN;
	END
	
	-- Verificar que el usuario no tenga una reserva pendiente o confirmada para el mismo tour
	IF EXISTS (SELECT 1 FROM dbo.TReserva WHERE IdTour = @IdTour AND IdUsuario = @IdUsuario AND EstadoReserva IN ('Pendiente', 'Confirmada'))
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -3 AS Resultado, 'Ya tienes una reserva activa para este tour' AS Mensaje;
		RETURN;
	END
	
	-- Calcular precio total
	DECLARE @PrecioTotal DECIMAL(10,2) = @PrecioUnitario * @CantidadPersonas;
	
	-- Crear la reserva
	INSERT INTO dbo.TReserva (IdTour, IdUsuario, CantidadPersonas, PrecioTotal, EstadoReserva, Comentarios)
	VALUES (@IdTour, @IdUsuario, @CantidadPersonas, @PrecioTotal, 'Pendiente', @Comentarios);
	
	DECLARE @IdReserva BIGINT = SCOPE_IDENTITY();
	
	COMMIT TRANSACTION;
	
	SELECT @IdReserva AS Resultado, 'Reserva creada exitosamente' AS Mensaje;
END;
GO

-- Procedimiento para consultar reservas de un usuario
CREATE PROCEDURE [dbo].[ConsultarReservasUsuario]
	@IdUsuario BIGINT
AS
BEGIN
	SELECT	R.IdReserva,
			R.IdTour,
			T.Nombre AS NombreTour,
			T.Destino,
			T.FechaInicio,
			T.FechaFin,
			R.CantidadPersonas,
			R.PrecioTotal,
			R.FechaReserva,
			R.EstadoReserva,
			R.Comentarios,
			U.Nombre AS NombreCreador
	FROM	dbo.TReserva R
	INNER JOIN dbo.TTour T ON R.IdTour = T.IdTour
	INNER JOIN dbo.TUsuario U ON T.IdUsuarioCreador = U.IdUsuario
	WHERE	R.IdUsuario = @IdUsuario
	ORDER BY R.FechaReserva DESC;
END;
GO

-- Procedimiento para cancelar una reserva
CREATE PROCEDURE [dbo].[CancelarReserva]
	@IdReserva BIGINT,
	@IdUsuario BIGINT
AS
BEGIN
	-- Verificar que la reserva pertenece al usuario y se puede cancelar
	IF NOT EXISTS (SELECT 1 FROM dbo.TReserva WHERE IdReserva = @IdReserva AND IdUsuario = @IdUsuario AND EstadoReserva IN ('Pendiente', 'Confirmada'))
	BEGIN
		SELECT -1 AS Resultado, 'No se puede cancelar esta reserva' AS Mensaje;
		RETURN;
	END
	
	-- Actualizar estado de la reserva
	UPDATE dbo.TReserva 
	SET EstadoReserva = 'Cancelada',
		FechaActualizacion = GETDATE()
	WHERE IdReserva = @IdReserva AND IdUsuario = @IdUsuario;
	
	SELECT 1 AS Resultado, 'Reserva cancelada exitosamente' AS Mensaje;
END;
GO

-- Procedimiento para obtener estadísticas de reservas (solo administradores)
CREATE PROCEDURE [dbo].[ObtenerEstadisticasReservas]
AS
BEGIN
	SELECT 
		COUNT(*) as TotalReservas,
		SUM(CASE WHEN EstadoReserva = 'Pendiente' THEN 1 ELSE 0 END) as ReservasPendientes,
		SUM(CASE WHEN EstadoReserva = 'Confirmada' THEN 1 ELSE 0 END) as ReservasConfirmadas,
		SUM(CASE WHEN EstadoReserva = 'Cancelada' THEN 1 ELSE 0 END) as ReservasCanceladas,
		SUM(CASE WHEN EstadoReserva IN ('Pendiente', 'Confirmada') THEN PrecioTotal ELSE 0 END) as IngresosTotales,
		SUM(CASE WHEN EstadoReserva IN ('Pendiente', 'Confirmada') THEN CantidadPersonas ELSE 0 END) as PersonasTotales
	FROM dbo.TReserva;
END;
GO

-- Agregar tabla de Eventos Sociales
CREATE TABLE [dbo].[TEventoSocial](
	[IdEventoSocial] BIGINT IDENTITY(1,1) NOT NULL,
	[Nombre] VARCHAR(255) NOT NULL,
	[Descripcion] TEXT NOT NULL,
	[Ubicacion] VARCHAR(255) NOT NULL,
	[Precio] DECIMAL(10,2) NOT NULL,
	[FechaInicio] DATETIME NOT NULL,
	[FechaFin] DATETIME NOT NULL,
	[CantidadPersonas] INT NOT NULL,
	[Estado] BIT NOT NULL DEFAULT 1,
	[FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
	[IdUsuarioCreador] BIGINT NOT NULL,
 CONSTRAINT [PK_TEventoSocial] PRIMARY KEY CLUSTERED ([IdEventoSocial] ASC)
);
GO

-- Agregar tabla de Eventos Universitarios
CREATE TABLE [dbo].[TEventoUniversitario](
	[IdEventoUniversitario] BIGINT IDENTITY(1,1) NOT NULL,
	[Nombre] VARCHAR(255) NOT NULL,
	[Descripcion] TEXT NOT NULL,
	[Ubicacion] VARCHAR(255) NOT NULL,
	[Universidad] VARCHAR(255) NOT NULL,
	[Precio] DECIMAL(10,2) NOT NULL,
	[FechaInicio] DATETIME NOT NULL,
	[FechaFin] DATETIME NOT NULL,
	[CantidadPersonas] INT NOT NULL,
	[Estado] BIT NOT NULL DEFAULT 1,
	[FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
	[IdUsuarioCreador] BIGINT NOT NULL,
 CONSTRAINT [PK_TEventoUniversitario] PRIMARY KEY CLUSTERED ([IdEventoUniversitario] ASC)
);
GO

-- Crear relaciones con usuario creador
ALTER TABLE [dbo].[TEventoSocial] WITH CHECK ADD  
CONSTRAINT [FK_TEventoSocial_TUsuario] 
FOREIGN KEY([IdUsuarioCreador]) REFERENCES [dbo].[TUsuario] ([IdUsuario]);
GO
ALTER TABLE [dbo].[TEventoSocial] CHECK CONSTRAINT [FK_TEventoSocial_TUsuario];
GO

ALTER TABLE [dbo].[TEventoUniversitario] WITH CHECK ADD  
CONSTRAINT [FK_TEventoUniversitario_TUsuario] 
FOREIGN KEY([IdUsuarioCreador]) REFERENCES [dbo].[TUsuario] ([IdUsuario]);
GO
ALTER TABLE [dbo].[TEventoUniversitario] CHECK CONSTRAINT [FK_TEventoUniversitario_TUsuario];
GO

-- ===============================
-- PROCEDIMIENTOS EVENTOS SOCIALES
-- ===============================

-- Procedimiento para consultar todos los eventos sociales
CREATE PROCEDURE [dbo].[ConsultarEventosSociales]
AS
BEGIN
	SELECT	ES.IdEventoSocial,
			ES.Nombre,
			ES.Descripcion,
			ES.Ubicacion,
			ES.Precio,
			ES.FechaInicio,
			ES.FechaFin,
			ES.CantidadPersonas,
			ES.Estado,
			ES.FechaCreacion,
			ES.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TEventoSocial ES
	INNER JOIN dbo.TUsuario U ON ES.IdUsuarioCreador = U.IdUsuario
	WHERE	ES.Estado = 1
	ORDER BY ES.FechaCreacion DESC;
END;
GO

-- Procedimiento para consultar un evento social por ID
CREATE PROCEDURE [dbo].[ConsultarEventoSocialPorId]
	@IdEventoSocial BIGINT
AS
BEGIN
	SELECT	ES.IdEventoSocial,
			ES.Nombre,
			ES.Descripcion,
			ES.Ubicacion,
			ES.Precio,
			ES.FechaInicio,
			ES.FechaFin,
			ES.CantidadPersonas,
			ES.Estado,
			ES.FechaCreacion,
			ES.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TEventoSocial ES
	INNER JOIN dbo.TUsuario U ON ES.IdUsuarioCreador = U.IdUsuario
	WHERE	ES.IdEventoSocial = @IdEventoSocial
		AND ES.Estado = 1;
END;
GO

-- Procedimiento para registrar nuevo evento social
CREATE PROCEDURE [dbo].[RegistrarEventoSocial]
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Ubicacion VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATETIME,
	@FechaFin DATETIME,
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

	-- Insertar el nuevo evento social
	INSERT INTO dbo.TEventoSocial (Nombre, Descripcion, Ubicacion, Precio, FechaInicio, FechaFin, CantidadPersonas, IdUsuarioCreador, Estado)
	VALUES (@Nombre, @Descripcion, @Ubicacion, @Precio, @FechaInicio, @FechaFin, @CantidadPersonas, @IdUsuarioCreador, 1)

	-- Devolver el ID del evento creado
	SELECT SCOPE_IDENTITY() AS Resultado, 'Evento social registrado exitosamente' AS Mensaje
END
GO

-- Procedimiento para actualizar evento social
CREATE PROCEDURE [dbo].[ActualizarEventoSocial]
	@IdEventoSocial BIGINT,
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Ubicacion VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATETIME,
	@FechaFin DATETIME,
	@CantidadPersonas INT
AS
BEGIN
	-- Validar que el evento existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TEventoSocial WHERE IdEventoSocial = @IdEventoSocial AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El evento social no existe o está inactivo' AS Mensaje
		RETURN
	END

	-- Validar que la fecha de fin sea mayor que la fecha de inicio
	IF @FechaFin <= @FechaInicio
	BEGIN
		SELECT -2 AS Resultado, 'La fecha de fin debe ser posterior a la fecha de inicio' AS Mensaje
		RETURN
	END

	-- Actualizar el evento
	UPDATE dbo.TEventoSocial 
	SET Nombre = @Nombre,
		Descripcion = @Descripcion,
		Ubicacion = @Ubicacion,
		Precio = @Precio,
		FechaInicio = @FechaInicio,
		FechaFin = @FechaFin,
		CantidadPersonas = @CantidadPersonas
	WHERE IdEventoSocial = @IdEventoSocial

	SELECT 1 AS Resultado, 'Evento social actualizado exitosamente' AS Mensaje
END
GO

-- Procedimiento para eliminar evento social (eliminación lógica)
CREATE PROCEDURE [dbo].[EliminarEventoSocial]
	@IdEventoSocial BIGINT
AS
BEGIN
	-- Validar que el evento existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TEventoSocial WHERE IdEventoSocial = @IdEventoSocial AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El evento social no existe o ya está eliminado' AS Mensaje
		RETURN
	END

	-- Eliminar lógicamente el evento
	UPDATE dbo.TEventoSocial 
	SET Estado = 0
	WHERE IdEventoSocial = @IdEventoSocial

	SELECT 1 AS Resultado, 'Evento social eliminado exitosamente' AS Mensaje
END
GO

-- ====================================
-- PROCEDIMIENTOS EVENTOS UNIVERSITARIOS
-- ====================================

-- Procedimiento para consultar todos los eventos universitarios
CREATE PROCEDURE [dbo].[ConsultarEventosUniversitarios]
AS
BEGIN
	SELECT	EU.IdEventoUniversitario,
			EU.Nombre,
			EU.Descripcion,
			EU.Ubicacion,
			EU.Universidad,
			EU.Precio,
			EU.FechaInicio,
			EU.FechaFin,
			EU.CantidadPersonas,
			EU.Estado,
			EU.FechaCreacion,
			EU.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TEventoUniversitario EU
	INNER JOIN dbo.TUsuario U ON EU.IdUsuarioCreador = U.IdUsuario
	WHERE	EU.Estado = 1
	ORDER BY EU.FechaCreacion DESC;
END;
GO

-- Procedimiento para consultar un evento universitario por ID
CREATE PROCEDURE [dbo].[ConsultarEventoUniversitarioPorId]
	@IdEventoUniversitario BIGINT
AS
BEGIN
	SELECT	EU.IdEventoUniversitario,
			EU.Nombre,
			EU.Descripcion,
			EU.Ubicacion,
			EU.Universidad,
			EU.Precio,
			EU.FechaInicio,
			EU.FechaFin,
			EU.CantidadPersonas,
			EU.Estado,
			EU.FechaCreacion,
			EU.IdUsuarioCreador,
			U.Nombre AS NombreCreador
	FROM	dbo.TEventoUniversitario EU
	INNER JOIN dbo.TUsuario U ON EU.IdUsuarioCreador = U.IdUsuario
	WHERE	EU.IdEventoUniversitario = @IdEventoUniversitario
		AND EU.Estado = 1;
END;
GO

-- Procedimiento para registrar nuevo evento universitario
CREATE PROCEDURE [dbo].[RegistrarEventoUniversitario]
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Ubicacion VARCHAR(255),
	@Universidad VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATETIME,
	@FechaFin DATETIME,
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

	-- Insertar el nuevo evento universitario
	INSERT INTO dbo.TEventoUniversitario (Nombre, Descripcion, Ubicacion, Universidad, Precio, FechaInicio, FechaFin, CantidadPersonas, IdUsuarioCreador, Estado)
	VALUES (@Nombre, @Descripcion, @Ubicacion, @Universidad, @Precio, @FechaInicio, @FechaFin, @CantidadPersonas, @IdUsuarioCreador, 1)

	-- Devolver el ID del evento creado
	SELECT SCOPE_IDENTITY() AS Resultado, 'Evento universitario registrado exitosamente' AS Mensaje
END
GO

-- Procedimiento para actualizar evento universitario
CREATE PROCEDURE [dbo].[ActualizarEventoUniversitario]
	@IdEventoUniversitario BIGINT,
	@Nombre VARCHAR(255),
	@Descripcion TEXT,
	@Ubicacion VARCHAR(255),
	@Universidad VARCHAR(255),
	@Precio DECIMAL(10,2),
	@FechaInicio DATETIME,
	@FechaFin DATETIME,
	@CantidadPersonas INT
AS
BEGIN
	-- Validar que el evento existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TEventoUniversitario WHERE IdEventoUniversitario = @IdEventoUniversitario AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El evento universitario no existe o está inactivo' AS Mensaje
		RETURN
	END

	-- Validar que la fecha de fin sea mayor que la fecha de inicio
	IF @FechaFin <= @FechaInicio
	BEGIN
		SELECT -2 AS Resultado, 'La fecha de fin debe ser posterior a la fecha de inicio' AS Mensaje
		RETURN
	END

	-- Actualizar el evento
	UPDATE dbo.TEventoUniversitario 
	SET Nombre = @Nombre,
		Descripcion = @Descripcion,
		Ubicacion = @Ubicacion,
		Universidad = @Universidad,
		Precio = @Precio,
		FechaInicio = @FechaInicio,
		FechaFin = @FechaFin,
		CantidadPersonas = @CantidadPersonas
	WHERE IdEventoUniversitario = @IdEventoUniversitario

	SELECT 1 AS Resultado, 'Evento universitario actualizado exitosamente' AS Mensaje
END
GO

-- Procedimiento para eliminar evento universitario (eliminación lógica)
CREATE PROCEDURE [dbo].[EliminarEventoUniversitario]
	@IdEventoUniversitario BIGINT
AS
BEGIN
	-- Validar que el evento existe
	IF NOT EXISTS (SELECT 1 FROM dbo.TEventoUniversitario WHERE IdEventoUniversitario = @IdEventoUniversitario AND Estado = 1)
	BEGIN
		SELECT -1 AS Resultado, 'El evento universitario no existe o ya está eliminado' AS Mensaje
		RETURN
	END

	-- Eliminar lógicamente el evento
	UPDATE dbo.TEventoUniversitario 
	SET Estado = 0
	WHERE IdEventoUniversitario = @IdEventoUniversitario

	SELECT 1 AS Resultado, 'Evento universitario eliminado exitosamente' AS Mensaje
END
GO


-- Crear tabla de reservas sociales
CREATE TABLE [dbo].[TReservaSocial](
	[IdReservaSocial] BIGINT IDENTITY(1,1) NOT NULL,
	[IdEventoSocial] BIGINT NOT NULL,
	[IdUsuario] BIGINT NOT NULL,
	[CantidadPersonas] INT NOT NULL,
	[PrecioTotal] DECIMAL(10,2) NOT NULL,
	[FechaReserva] DATETIME NOT NULL DEFAULT GETDATE(),
	[EstadoReserva] VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
	[Comentarios] VARCHAR(500) NULL,
	[FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
	[FechaActualizacion] DATETIME NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_TReservaSocial] PRIMARY KEY CLUSTERED ([IdReservaSocial] ASC)
);
GO

-- Crear relaciones
ALTER TABLE [dbo].[TReservaSocial]  WITH CHECK ADD  
CONSTRAINT [FK_TReservaSocial_TEventoSocial] 
FOREIGN KEY([IdEventoSocial]) REFERENCES [dbo].[TEventoSocial] ([IdEventoSocial]);
GO

ALTER TABLE [dbo].[TReservaSocial]  WITH CHECK ADD  
CONSTRAINT [FK_TReservaSocial_TUsuario] 
FOREIGN KEY([IdUsuario]) REFERENCES [dbo].[TUsuario] ([IdUsuario]);
GO

ALTER TABLE [dbo].[TReservaSocial] CHECK CONSTRAINT [FK_TReservaSocial_TEventoSocial];
GO
ALTER TABLE [dbo].[TReservaSocial] CHECK CONSTRAINT [FK_TReservaSocial_TUsuario];
GO

-- Procedimiento para consultar eventos sociales disponibles para reserva
CREATE PROCEDURE [dbo].[ConsultarEventosSocialesDisponibles]
AS
BEGIN
	SELECT	ES.IdEventoSocial,
			ES.Nombre,
			CAST(ES.Descripcion AS VARCHAR(MAX)) AS Descripcion,
			ES.Ubicacion,
			ES.Precio,
			ES.FechaInicio,
			ES.FechaFin,
			ES.CantidadPersonas,
			ES.FechaCreacion,
			U.Nombre AS NombreCreador,
			ISNULL(SUM(RS.CantidadPersonas), 0) AS PersonasReservadas,
			(ES.CantidadPersonas - ISNULL(SUM(RS.CantidadPersonas), 0)) AS CuposDisponibles
	FROM	dbo.TEventoSocial ES
	INNER JOIN dbo.TUsuario U ON ES.IdUsuarioCreador = U.IdUsuario
	LEFT JOIN dbo.TReservaSocial RS ON ES.IdEventoSocial = RS.IdEventoSocial AND RS.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE	ES.Estado = 1
		AND ES.FechaInicio > GETDATE()
	GROUP BY ES.IdEventoSocial, ES.Nombre, CAST(ES.Descripcion AS VARCHAR(MAX)), ES.Ubicacion, ES.Precio, 
			 ES.FechaInicio, ES.FechaFin, ES.CantidadPersonas, ES.FechaCreacion, U.Nombre
	HAVING (ES.CantidadPersonas - ISNULL(SUM(RS.CantidadPersonas), 0)) > 0
	ORDER BY ES.FechaInicio ASC;
END;
GO

-- Procedimiento para consultar un evento social específico con disponibilidad
CREATE PROCEDURE [dbo].[ConsultarEventoSocialDisponiblePorId]
	@IdEventoSocial BIGINT
AS
BEGIN
	SELECT	ES.IdEventoSocial,
			ES.Nombre,
			CAST(ES.Descripcion AS VARCHAR(MAX)) AS Descripcion,
			ES.Ubicacion,
			ES.Precio,
			ES.FechaInicio,
			ES.FechaFin,
			ES.CantidadPersonas,
			ES.FechaCreacion,
			U.Nombre AS NombreCreador,
			ISNULL(SUM(RS.CantidadPersonas), 0) AS PersonasReservadas,
			(ES.CantidadPersonas - ISNULL(SUM(RS.CantidadPersonas), 0)) AS CuposDisponibles
	FROM	dbo.TEventoSocial ES
	INNER JOIN dbo.TUsuario U ON ES.IdUsuarioCreador = U.IdUsuario
	LEFT JOIN dbo.TReservaSocial RS ON ES.IdEventoSocial = RS.IdEventoSocial AND RS.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE	ES.IdEventoSocial = @IdEventoSocial
		AND ES.Estado = 1
		AND ES.FechaInicio > GETDATE()
	GROUP BY ES.IdEventoSocial, ES.Nombre, CAST(ES.Descripcion AS VARCHAR(MAX)), ES.Ubicacion, ES.Precio, 
			 ES.FechaInicio, ES.FechaFin, ES.CantidadPersonas, ES.FechaCreacion, U.Nombre;
END;
GO

-- Procedimiento para crear una reserva de evento social
CREATE PROCEDURE [dbo].[CrearReservaEventoSocial]
	@IdEventoSocial BIGINT,
	@IdUsuario BIGINT,
	@CantidadPersonas INT,
	@Comentarios VARCHAR(500) = NULL
AS
BEGIN
	BEGIN TRANSACTION;
	
	DECLARE @PrecioUnitario DECIMAL(10,2);
	DECLARE @CuposDisponibles INT;
	DECLARE @EventoValido BIT = 0;
	
	-- Verificar si el evento existe y está disponible
	SELECT @PrecioUnitario = ES.Precio,
		   @CuposDisponibles = (ES.CantidadPersonas - ISNULL(SUM(RS.CantidadPersonas), 0)),
		   @EventoValido = 1
	FROM dbo.TEventoSocial ES
	LEFT JOIN dbo.TReservaSocial RS ON ES.IdEventoSocial = RS.IdEventoSocial AND RS.EstadoReserva IN ('Pendiente', 'Confirmada')
	WHERE ES.IdEventoSocial = @IdEventoSocial 
		AND ES.Estado = 1 
		AND ES.FechaInicio > GETDATE()
	GROUP BY ES.IdEventoSocial, ES.Precio, ES.CantidadPersonas;
	
	IF @EventoValido = 0
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -1 AS Resultado, 'El evento no existe o no está disponible para reserva' AS Mensaje;
		RETURN;
	END
	
	-- Verificar disponibilidad de cupos
	IF @CuposDisponibles < @CantidadPersonas
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -2 AS Resultado, 'No hay suficientes cupos disponibles. Cupos disponibles: ' + CAST(@CuposDisponibles AS VARCHAR(10)) AS Mensaje;
		RETURN;
	END
	
	-- Verificar que el usuario no tenga una reserva pendiente o confirmada para el mismo evento
	IF EXISTS (SELECT 1 FROM dbo.TReservaSocial WHERE IdEventoSocial = @IdEventoSocial AND IdUsuario = @IdUsuario AND EstadoReserva IN ('Pendiente', 'Confirmada'))
	BEGIN
		ROLLBACK TRANSACTION;
		SELECT -3 AS Resultado, 'Ya tienes una reserva activa para este evento' AS Mensaje;
		RETURN;
	END
	
	-- Calcular precio total
	DECLARE @PrecioTotal DECIMAL(10,2) = @PrecioUnitario * @CantidadPersonas;
	
	-- Crear la reserva
	INSERT INTO dbo.TReservaSocial (IdEventoSocial, IdUsuario, CantidadPersonas, PrecioTotal, EstadoReserva, Comentarios)
	VALUES (@IdEventoSocial, @IdUsuario, @CantidadPersonas, @PrecioTotal, 'Pendiente', @Comentarios);
	
	DECLARE @IdReservaSocial BIGINT = SCOPE_IDENTITY();
	
	COMMIT TRANSACTION;
	
	SELECT @IdReservaSocial AS Resultado, 'Reserva de evento social creada exitosamente' AS Mensaje;
END;
GO

-- Procedimiento para consultar reservas de eventos sociales de un usuario
CREATE PROCEDURE [dbo].[ConsultarReservasEventoSocialUsuario]
	@IdUsuario BIGINT
AS
BEGIN
	SELECT	RS.IdReservaSocial,
			RS.IdEventoSocial,
			ES.Nombre AS NombreEvento,
			ES.Ubicacion,
			ES.FechaInicio,
			ES.FechaFin,
			RS.CantidadPersonas,
			RS.PrecioTotal,
			RS.FechaReserva,
			RS.EstadoReserva,
			RS.Comentarios,
			U.Nombre AS NombreCreador
	FROM	dbo.TReservaSocial RS
	INNER JOIN dbo.TEventoSocial ES ON RS.IdEventoSocial = ES.IdEventoSocial
	INNER JOIN dbo.TUsuario U ON ES.IdUsuarioCreador = U.IdUsuario
	WHERE	RS.IdUsuario = @IdUsuario
	ORDER BY RS.FechaReserva DESC;
END;
GO

-- Procedimiento para cancelar una reserva de evento social
CREATE PROCEDURE [dbo].[CancelarReservaEventoSocial]
	@IdReservaSocial BIGINT,
	@IdUsuario BIGINT
AS
BEGIN
	-- Verificar que la reserva pertenece al usuario y se puede cancelar
	IF NOT EXISTS (SELECT 1 FROM dbo.TReservaSocial WHERE IdReservaSocial = @IdReservaSocial AND IdUsuario = @IdUsuario AND EstadoReserva IN ('Pendiente', 'Confirmada'))
	BEGIN
		SELECT -1 AS Resultado, 'No se puede cancelar esta reserva' AS Mensaje;
		RETURN;
	END
	
	-- Actualizar estado de la reserva
	UPDATE dbo.TReservaSocial 
	SET EstadoReserva = 'Cancelada',
		FechaActualizacion = GETDATE()
	WHERE IdReservaSocial = @IdReservaSocial AND IdUsuario = @IdUsuario;
	
	SELECT 1 AS Resultado, 'Reserva de evento social cancelada exitosamente' AS Mensaje;
END;
GO
