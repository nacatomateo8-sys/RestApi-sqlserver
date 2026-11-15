namespace RestApi.Extensiones;

using RestApi.Datos;
using RestApi.Datos.Interfaces;
using RestApi.Negocio;

public static class ServiciosRestaurante
{
    public static IServiceCollection AddRestaurante(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPlatoRepositorio, PlatoRepositorio>();
        services.AddScoped<PlatoServicio>();

        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<CategoriaServicio>();

        services.AddScoped<IMesaRepositorio, MesaRepositorio>();
        services.AddScoped<MesaServicio>();

        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<UsuarioServicio>();

        services.AddScoped<TokenServicio>();
        services.AddScoped<AuthServicio>();

        return services;
    }
}
