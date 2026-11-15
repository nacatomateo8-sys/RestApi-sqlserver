# Ejemplo Práctico de Revisión y Comentarios

Este ejemplo ilustra cómo aplicar la habilidad `code-reviewer-commenter` sobre un método de acceso a datos en C# con ADO.NET.

---

### Código Original (Antes de la revisión)

```csharp
public async Task<int> Agregar(Plato p)
{
    var cn = new SqlConnection(_cn);
    cn.Open();
    var cmd = new SqlCommand("INSERT INTO Platos (Nombre, Precio) VALUES ('" + p.Nombre + "', " + p.Precio + ")", cn);
    return (int)cmd.ExecuteScalar();
}
```

---

### Análisis Línea por Línea

| Línea | ¿Qué hace? | Estado | Diagnóstico y Sugerencia |
| :--- | :--- | :---: | :--- |
| `public async Task<int> Agregar(Plato p)` | Firma del método asíncrono para crear plato. | `[⚠] Mejorable` | Según convenciones de C# y `AGENTS.md`, los métodos asíncronos deben terminar con el sufijo `Async` (`AgregarAsync`). |
| `var cn = new SqlConnection(_cn);` | Instancia la conexión a SQL Server. | `[✗] Riesgo` | No usa `await using`. Si ocurre un error, la conexión no se libera de inmediato, provocando fugas en el connection pool. |
| `cn.Open();` | Abre la conexión de forma sincrónica bloqueante. | `[✗] Error` | Bloquea el hilo del servidor. Debe utilizar `await cn.OpenAsync();`. |
| `var cmd = new SqlCommand("... '" + p.Nombre + ...", cn);` | Crea comando SQL concatenando texto directo. | `[✗] Crítico` | **Vulnerabilidad de Inyección SQL**. Nunca concatenar variables en consultas SQL. Debe usar parámetros (`@nombre`, `@precio`). |
| `return (int)cmd.ExecuteScalar();` | Ejecuta la consulta de forma sincrónica y castea el resultado. | `[✗] Error` | Debe usar `await cmd.ExecuteScalarAsync()` y convertir de forma segura con `Convert.ToInt32` para evitar `NullReferenceException` o `InvalidCastException`. |

---

### Código Corregido y Comentado

```csharp
/// <summary>
/// Inserta un nuevo plato en la base de datos de manera asíncrona y segura.
/// </summary>
/// <param name="p">Entidad Plato con los datos validados previamente.</param>
/// <returns>Identificador autogenerado por la base de datos (SCOPE_IDENTITY / OUTPUT INSERTED.Id).</returns>
public async Task<int> AgregarAsync(Plato p)
{
    // Liberación asíncrona garantizada de la conexión al terminar el bloque (previene fugas de recursos)
    await using var cn = new SqlConnection(_cn);
    await cn.OpenAsync();

    // Consulta parametrizada para prevenir inyección SQL y obtener de inmediato el Id generado
    await using var cmd = new SqlCommand(
        @"INSERT INTO Platos (Nombre, Precio)
          OUTPUT INSERTED.Id
          VALUES (@nombre, @precio)", cn);

    // Asociación de parámetros fuertemente tipados
    cmd.Parameters.AddWithValue("@nombre", p.Nombre);
    cmd.Parameters.AddWithValue("@precio", p.Precio);

    // Conversión segura del Id escalar devuelto por SQL Server
    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
}
```
