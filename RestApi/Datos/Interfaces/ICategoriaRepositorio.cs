namespace RestApi.Datos.Interfaces;

using RestApi.Entidades;

// Corrección: Interfaz para desacoplar la capa de negocio de la implementación de datos SQL Server (según AGENTS.md)
public interface ICategoriaRepositorio
{
    Task<List<Categoria>> ObtenerTodasAsync();
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task<int> AgregarAsync(Categoria c);
    Task<int> ActualizarAsync(Categoria c);
    Task<int> EliminarAsync(int id);
    Task<int> ContarPlatosAsync(int id);
}
