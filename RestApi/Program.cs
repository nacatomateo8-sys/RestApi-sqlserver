using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestApi.Endpoints;
using RestApi.Entidades;
using RestApi.Extensiones;

// 0. Cargar variables de entorno desde .env antes de inicializar la aplicación
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de dependencias (repositorios y servicios de negocio)
builder.Services.AddRestaurante(builder.Configuration);

// 2. Configuración de autenticación JWT
var jwt = builder.Configuration.GetSection("Jwt");
var clave = jwt["Key"];
if (string.IsNullOrWhiteSpace(clave) || clave.Length < 32)
    throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

// 3. Configuración de políticas de autorización
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SoloAdmin", p => p.RequireRole(Roles.Administrador));

var app = builder.Build();

// 4. Middlewares de seguridad (en orden estricto de ejecución)
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapeo de endpoints por módulo
app.MapPlatoEndpoints();
app.MapCategoriaEndpoints();
app.MapMesaEndpoints();
app.MapUsuarioEndpoints();
app.MapAuthEndpoints();

app.Run();
