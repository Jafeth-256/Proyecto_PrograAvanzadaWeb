using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proyecto_ProgaAvanzadaWeb_API.Helpers;
using Proyecto_ProgaAvanzadaWeb_API.Services;
using Proyecto_PrograAvanzadaWeb_API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger con soporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TicoTours API",
        Version = "v1",
        Description = "API para gestión de tours y usuarios"
    });

    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// CORS configuration mejorado - ACTUALIZADO
builder.Services.AddCors(options =>
{
    // Política para desarrollo (permite todo)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Política específica para la aplicación MVC - NUEVA
    options.AddPolicy("AllowMvcApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7273", "http://localhost:7273",  // MVC HTTPS y HTTP
                "https://localhost:7001", "http://localhost:5001"   // Por si API y MVC están en mismo puerto
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();// Importante para autenticación
    });
});

// Database Context
builder.Services.AddSingleton<DataContext>();

// Helpers
builder.Services.AddScoped<JwtHelper>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();

// JWT Authentication configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TicoTours API v1");
        c.RoutePrefix = string.Empty; // Para que Swagger sea la página principal
    });

    // Usar política permisiva en desarrollo - ACTUALIZADO
    app.UseCors("AllowMvcApp");
}
else
{
    // En producción usar política restrictiva - NUEVO
    app.UseCors("Production");
}

app.UseHttpsRedirection();

// Importante: El orden de los middlewares es crucial
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();