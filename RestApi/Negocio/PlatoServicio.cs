namespace RestApi.Negocio;

using RestApi.Datos;
using RestApi.Datos.Interfaces;
using RestApi.Entidades;

public class PlatoServicio(IPlatoRepositorio repo, CategoriaRepositorio categorias)
{
    public Task<List<Plato>> ObtenerTodosAsync() => repo.ObtenerTodosAsync();

    public async Task<Plato> ObtenerAsync(int id)
    {
        var plato = await repo.ObtenerPorIdAsync(id);
        if (plato is null)
            throw new KeyNotFoundException($"No existe el plato {id}.");                  // 404
        return plato;
    }

    public async Task<List<Plato>> ObtenerPorCategoriaAsync(int categoriaId)
    {
        if (await categorias.ObtenerPorIdAsync(categoriaId) is null)
            throw new KeyNotFoundException($"No existe la categoría {categoriaId}.");     // 404
        return await repo.ObtenerPorCategoriaAsync(categoriaId);
    }

    public async Task<int> AgregarAsync(Plato p)
    {
        await Validar(p);
        return await repo.AgregarAsync(p);
    }

    public async Task ActualizarAsync(Plato p)
    {
        await Validar(p);                                                                 // 400
        if (!await repo.ActualizarAsync(p))
            throw new KeyNotFoundException($"No existe el plato {p.Id}.");                // 404
    }

    public async Task EliminarAsync(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException($"No existe el plato {id}.");                  // 404
    }

    // Validación compartida por crear y actualizar
    private async Task Validar(Plato p)
    {
        if (string.IsNullOrWhiteSpace(p.Nombre))
            throw new ArgumentException("El nombre del plato es obligatorio.");
        if (p.Nombre.Length > 100)
            throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
        if (p.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");
        if (await categorias.ObtenerPorIdAsync(p.CategoriaId) is null)
            throw new ArgumentException($"La categoría {p.CategoriaId} no existe.");
        p.Nombre = p.Nombre.Trim();
        p.Descripcion = (p.Descripcion ?? "").Trim();
    }
}