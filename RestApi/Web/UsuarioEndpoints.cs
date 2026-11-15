namespace RestApi.Endpoints;

using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/usuarios").RequireAuthorization("SoloAdmin");

        g.MapGet("", async (UsuarioServicio s) =>
            Results.Ok(await s.ListarAsync()));

        g.MapGet("/{id:int}", (int id, UsuarioServicio s) =>
            Respuestas.Manejar(async () => Results.Ok(await s.ObtenerPorIdAsync(id))));

        g.MapPost("", (RegistroUsuario r, UsuarioServicio s) =>
            Respuestas.Manejar(async () =>
            {
                var u = await s.RegistrarAsync(r);
                return Results.Created($"/usuarios/{u.Id}", u);
            }));
    }
}
