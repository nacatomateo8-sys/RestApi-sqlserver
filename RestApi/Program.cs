using RestApi.Endpoints;
using RestApi.Extensiones;

// 0. Cargar .env ANTES de crear el builder
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar las dependencias y servicios
builder.Services.AddRestaurante(builder.Configuration);

var app = builder.Build();

// 2. Mapear endpoints por módulo
app.MapPlatoEndpoints();
app.MapCategoriaEndpoints();
app.MapMesaEndpoints();

app.Run();
