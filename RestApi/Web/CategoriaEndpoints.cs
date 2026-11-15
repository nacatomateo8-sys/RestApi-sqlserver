namespace RestApi.Endpoints;

using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

public static class CategoriaEndpoints
{
    public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/categorias");

        // Lectura pública
        grupo.MapGet("/", async (CategoriaServicio s) =>
            Results.Ok(await s.ObtenerTodasAsync()));

        grupo.MapGet("/{id:int}", (int id, CategoriaServicio s) =>
            Respuestas.Manejar(async () => Results.Ok(await s.ObtenerPorIdAsync(id))));

        // Todos los platos de una categoría
        grupo.MapGet("/{id:int}/platos", (int id, PlatoServicio s) =>
            Respuestas.Manejar(async () => Results.Ok(await s.ObtenerPorCategoriaAsync(id))));

        // Modificaciones protegidas: Solo Administrador
        grupo.MapPost("/", (Categoria c, CategoriaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                c.Id = await s.AgregarAsync(c);
                return Results.Created($"/categorias/{c.Id}", c);
            }))
            .RequireAuthorization("SoloAdmin");

        grupo.MapPut("/{id:int}", (int id, Categoria c, CategoriaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                c.Id = id;
                await s.ActualizarAsync(c);
                return Results.Ok(c);
            }))
            .RequireAuthorization("SoloAdmin");

        grupo.MapDelete("/{id:int}", (int id, CategoriaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                await s.EliminarAsync(id);
                return Results.NoContent();
            }))
            .RequireAuthorization("SoloAdmin");
    }
}
