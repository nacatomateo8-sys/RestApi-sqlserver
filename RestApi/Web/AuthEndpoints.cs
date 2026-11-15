namespace RestApi.Endpoints;

using System.Security.Claims;
using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/auth");

        // LOGIN: público → 200 con token, o 401
        g.MapPost("/login", async (LoginRequest r, AuthServicio s) =>
            await s.LoginAsync(r) is { } res
                ? Results.Ok(res)
                : Results.Unauthorized());

        // REGISTRO: solo Administrador → 201 · 400 · 409 · 401 sin token · 403 sin permiso
        g.MapPost("/registro", (RegistroUsuario r, UsuarioServicio s) =>
            Respuestas.Manejar(async () =>
            {
                var u = await s.RegistrarAsync(r);
                return Results.Created($"/usuarios/{u.Id}", u);
            }))
        .RequireAuthorization("SoloAdmin");

        // YO: identidad del token actual
        g.MapGet("/yo", (ClaimsPrincipal user) => Results.Ok(new
        {
            id = user.FindFirstValue("sub"),
            nombre = user.Identity?.Name,
            rol = user.FindFirstValue("role")
        }))
        .RequireAuthorization();
    }
}
