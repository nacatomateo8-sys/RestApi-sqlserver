using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

namespace RestApi.Endpoints;

public static class PlatoEndpoints
{
    public static void MapPlatoEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/platos");

        // Lecturas públicas
        grupo.MapGet("/", async (PlatoServicio s) =>
            Results.Ok(await s.ObtenerTodosAsync()));

        grupo.MapGet("/{id:int}", (int id, PlatoServicio s) =>
            Respuestas.Manejar(async () => Results.Ok(await s.ObtenerAsync(id))));

        // Modificaciones protegidas: Solo Administrador
        grupo.MapPost("/", (Plato p, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                p.Id = await s.AgregarAsync(p);
                return Results.Created($"/platos/{p.Id}", p);
            }))
            .RequireAuthorization("SoloAdmin");

        grupo.MapPut("/{id:int}", (int id, Plato p, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                p.Id = id;
                await s.ActualizarAsync(p);
                return Results.Ok(p);
            }))
            .RequireAuthorization("SoloAdmin");

        grupo.MapDelete("/{id:int}", (int id, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                await s.EliminarAsync(id);
                return Results.NoContent();      // 204
            }))
            .RequireAuthorization("SoloAdmin");
    }
}
