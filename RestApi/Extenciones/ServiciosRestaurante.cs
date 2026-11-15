namespace RestApi.Extensiones;

using RestApi.Datos;
using RestApi.Datos.Interfaces;
using RestApi.Negocio;

public static class ServiciosRestaurante
{
    public static IServiceCollection AddRestaurante(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IPlatoRepositorio, PlatoRepositorio>();
        services.AddScoped<PlatoServicio>();

        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<CategoriaServicio>();

        services.AddScoped<IMesaRepositorio, MesaRepositorio>();
        services.AddScoped<MesaServicio>();

        services.AddScoped<ProductoRepositorio>();
        services.AddScoped<ProductoServicio>();

        return services;
    }
}
