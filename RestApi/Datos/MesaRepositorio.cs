namespace RestApi.Datos;

using Microsoft.Data.SqlClient;
using RestApi.Datos.Interfaces;
using RestApi.Entidades;

public class MesaRepositorio(IConfiguration config) : IMesaRepositorio
{
    private readonly string _cn = config.GetConnectionString("Restaurante")
        ?? throw new InvalidOperationException(
            "Falta la cadena de conexión 'Restaurante'. Crea el archivo .env (ver .env.example).");

    public async Task<List<Mesa>> ObtenerTodasAsync()
    {
        var lista = new List<Mesa>();
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "SELECT Id, Numero, Capacidad FROM Mesas ORDER BY Numero", cn);
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            lista.Add(new Mesa
            {
                Id = dr.GetInt32(0),
                Numero = dr.GetInt32(1),
                Capacidad = dr.GetInt32(2)
            });
        }
        return lista;
    }

    public async Task<Mesa?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "SELECT Id, Numero, Capacidad FROM Mesas WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;
        return new Mesa
        {
            Id = dr.GetInt32(0),
            Numero = dr.GetInt32(1),
            Capacidad = dr.GetInt32(2)
        };
    }

    public async Task<bool> ExisteNumeroAsync(int numero, int idExcluir)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Mesas WHERE Numero = @numero AND Id != @id", cn);
        cmd.Parameters.AddWithValue("@numero", numero);
        cmd.Parameters.AddWithValue("@id", idExcluir);
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }

    public async Task<int> AgregarAsync(Mesa m)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "INSERT INTO Mesas (Numero, Capacidad) OUTPUT INSERTED.Id VALUES (@numero, @capacidad)", cn);
        cmd.Parameters.AddWithValue("@numero", m.Numero);
        cmd.Parameters.AddWithValue("@capacidad", m.Capacidad);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<bool> ActualizarAsync(Mesa m)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "UPDATE Mesas SET Numero = @numero, Capacidad = @capacidad WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@id", m.Id);
        cmd.Parameters.AddWithValue("@numero", m.Numero);
        cmd.Parameters.AddWithValue("@capacidad", m.Capacidad);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_cn);
        await cn.OpenAsync();
        await using var cmd = new SqlCommand(
            "DELETE FROM Mesas WHERE Id = @id", cn);
        cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}
