namespace RestApi.Datos;

using Microsoft.Data.SqlClient;
using RestApi.Datos.Interfaces;
using RestApi.Entidades;

public class UsuarioRepositorio(IConfiguration config) : IUsuarioRepositorio
{
    private readonly string _cn = config.GetConnectionString("Restaurante")
        ?? throw new InvalidOperationException("Falta la cadena de conexión 'Restaurante'.");

    private static Usuario Leer(SqlDataReader dr) => new()
    {
        Id = dr.GetInt32(0),
        Nombre = dr.GetString(1),
        Correo = dr.GetString(2),
        PasswordHash = dr.GetString(3),
        Rol = dr.GetString(4),
        Activo = dr.GetBoolean(5)
    };

    public async Task<List<Usuario>> ListarAsync()
    {
        var lista = new List<Usuario>();
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, Nombre, Correo, PasswordHash, Rol, Activo FROM Usuarios WHERE Activo = 1 ORDER BY Nombre", cn);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync()) lista.Add(Leer(dr));
        return lista;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, Nombre, Correo, PasswordHash, Rol, Activo FROM Usuarios WHERE Id = @id AND Activo = 1", cn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Leer(dr) : null;
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, Nombre, Correo, PasswordHash, Rol, Activo FROM Usuarios WHERE Correo = @correo AND Activo = 1", cn);
        cmd.Parameters.AddWithValue("@correo", correo);

        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Leer(dr) : null;
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Usuarios WHERE Correo = @correo", cn);
        cmd.Parameters.AddWithValue("@correo", correo);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public async Task AgregarAsync(Usuario u)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new SqlCommand(
            @"INSERT INTO Usuarios (Nombre, Correo, PasswordHash, Rol, Activo)
              OUTPUT INSERTED.Id
              VALUES (@nombre, @correo, @hash, @rol, @activo)", cn);

        cmd.Parameters.AddWithValue("@nombre", u.Nombre);
        cmd.Parameters.AddWithValue("@correo", u.Correo);
        cmd.Parameters.AddWithValue("@hash", u.PasswordHash);
        cmd.Parameters.AddWithValue("@rol", u.Rol);
        cmd.Parameters.AddWithValue("@activo", u.Activo);

        try
        {
            u.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)   // violación de UNIQUE
        {
            throw new InvalidOperationException("Ese correo ya está registrado.");
        }
    }
}
