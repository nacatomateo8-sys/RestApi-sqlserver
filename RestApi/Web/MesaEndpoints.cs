namespace RestApi.Endpoints;

using QRCoder;
using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

public static class MesaEndpoints
{
    public static void MapMesaEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/mesas");

        // Lectura pública
        g.MapGet("", async (MesaServicio s) =>
            Results.Ok(await s.ListarAsync()));

        g.MapGet("/{id:int}", (int id, MesaServicio s) =>
            Respuestas.Manejar(async () =>
                Results.Ok(await s.ObtenerAsync(id))));

        // Modificaciones protegidas: Solo Administrador
        g.MapPost("", (Mesa m, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                m.Id = await s.CrearAsync(m);
                return Results.Created($"/mesas/{m.Id}", m);
            }))
            .RequireAuthorization("SoloAdmin");

        g.MapPut("/{id:int}", (int id, Mesa m, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                m.Id = id;
                await s.ActualizarAsync(m);
                return Results.Ok(m);
            }))
            .RequireAuthorization("SoloAdmin");

        g.MapDelete("/{id:int}", (int id, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                await s.EliminarAsync(id);
                return Results.NoContent();
            }))
            .RequireAuthorization("SoloAdmin");

        // Generación de código QR (público)
        g.MapGet("/{id:int}/qr", (int id, MesaServicio s, IConfiguration cfg) =>
            Respuestas.Manejar(async () =>
            {
                var mesa = await s.ObtenerAsync(id);
                var baseUrl = cfg["App:UrlMenu"] ?? "http://localhost:5205/menu";
                var url = $"{baseUrl}?mesa={mesa.Numero}";

                using var gen = new QRCodeGenerator();
                using var datos = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var png = new PngByteQRCode(datos).GetGraphic(10);

                return Results.File(png, "image/png", $"mesa-{mesa.Numero}.png");
            }));
    }
}
