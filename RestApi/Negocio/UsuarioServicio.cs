namespace RestApi.Negocio;

using RestApi.Datos.Interfaces;
using RestApi.Entidades;

public class UsuarioServicio(IUsuarioRepositorio repo)
{
    public async Task<List<UsuarioDto>> ListarAsync() =>
        (await repo.ListarAsync()).Select(ADto).ToList();

    public async Task<UsuarioDto> ObtenerPorIdAsync(int id)
    {
        var u = await repo.ObtenerPorIdAsync(id);
        if (u is null)
            throw new KeyNotFoundException($"No existe el usuario con Id {id}.");
        return ADto(u);
    }

    public async Task<UsuarioDto> RegistrarAsync(RegistroUsuario r)
    {
        // 1. Validaciones → ArgumentException (400)
        if (string.IsNullOrWhiteSpace(r.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(r.Correo) || !r.Correo.Contains('@'))
            throw new ArgumentException("El correo no es válido.");

        if (string.IsNullOrWhiteSpace(r.Password) || r.Password.Length < 8)
            throw new ArgumentException("La contraseña debe tener mínimo 8 caracteres.");

        if (!Roles.Todos.Contains(r.Rol))
            throw new ArgumentException("Rol inválido. Permitidos: Administrador, Mesero o Cocina.");

        // 2. Normalizar correo y revisar duplicado → InvalidOperationException (409)
        var correo = r.Correo.Trim().ToLower();
        if (await repo.ExisteCorreoAsync(correo))
            throw new InvalidOperationException("Ese correo ya está registrado.");

        // 3. Guardar solo el hash BCrypt
        var u = new Usuario
        {
            Nombre = r.Nombre.Trim(),
            Correo = correo,
            Rol = r.Rol,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password),
            Activo = true
        };

        await repo.AgregarAsync(u);
        return ADto(u);
    }

    private static UsuarioDto ADto(Usuario u) => new(u.Id, u.Nombre, u.Correo, u.Rol);
}
