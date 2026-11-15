using RestApi.Entidades;

namespace RestApi.Datos.Interfaces;

public interface IUsuarioRepositorio
{
    Task<List<Usuario>> ListarAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<bool> ExisteCorreoAsync(string correo);
    Task AgregarAsync(Usuario u);
}
