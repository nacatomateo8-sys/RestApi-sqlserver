using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestApi.Endpoints;
using RestApi.Entidades;
using RestApi.Extensiones;
using RestApi.Web;

// 0. Cargar .env ANTES de crear el builder
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// 1. Repositorios y servicios (incluye TokenServicio y AuthServicio)
builder.Services.AddRestaurante(builder.Configuration);

// 2. Autenticación JWT
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

// 3. Autorización: política SoloAdmin
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SoloAdmin", p => p.RequireRole(Roles.Administrador));

var app = builder.Build();

// 4. Middlewares (en este orden)
app.UseAuthentication();
app.UseAuthorization();

// 5. Endpoints
app.MapPlatoEndpoints();
app.MapCategoriaEndpoints();
app.MapMesaEndpoints();
app.MapUsuarioEndpoints();
app.MapAuthEndpoints();

app.Run();