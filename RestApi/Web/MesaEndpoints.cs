using QRCoder;
using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;

namespace RestApi.Endpoints;

public static class MesaEndpoints
{
    public static void MapMesaEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/mesas");

        g.MapGet("", async (MesaServicio s) =>
            Results.Ok(await s.ListarAsync()));

        g.MapGet("/{id:int}", (int id, MesaServicio s) =>
            Respuestas.Manejar(async () =>
                Results.Ok(await s.ObtenerAsync(id))));

        g.MapPost("", (Mesa m, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                m.Id = await s.CrearAsync(m);
                return Results.Created($"/mesas/{m.Id}", m);
            }));

        g.MapPut("/{id:int}", (int id, Mesa m, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                m.Id = id;
                await s.ActualizarAsync(m);
                return Results.Ok(m);
            }));

        g.MapDelete("/{id:int}", (int id, MesaServicio s) =>
            Respuestas.Manejar(async () =>
            {
                await s.EliminarAsync(id);
                return Results.NoContent();
            }));

        g.MapGet("/{id:int}/qr", (int id, MesaServicio s, IConfiguration cfg) =>
            Respuestas.Manejar(async () =>
            {
                var mesa = await s.ObtenerAsync(id);
                var url = $"{cfg["App:UrlMenu"]}?mesa={mesa.Numero}";

                using var gen = new QRCodeGenerator();
                using var datos = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var png = new PngByteQRCode(datos).GetGraphic(10);

                return Results.File(png, "image/png", $"mesa-{mesa.Numero}.png");
            }));
    }
}
