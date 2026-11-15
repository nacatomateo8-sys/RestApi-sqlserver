---
name: revision-restapi
description: >-
  Guía y procedimiento paso a paso para la revisión, auditoría técnica, control de calidad y refactorización de endpoints y capas en una REST API (ASP.NET Core Minimal APIs, SQL Server con ADO.NET).
---

# Skill: Revisión de REST API (`revision-restapi`)

## Nombre
`revision-restapi`

## Descripción
Esta habilidad define el protocolo técnico y los criterios de evaluación para auditar, revisar y validar componentes de una **REST API** construida con **ASP.NET Core (Minimal APIs)** y **SQL Server**. Proporciona una metodología estructurada para garantizar el cumplimiento de los estándares RESTful, la arquitectura por capas, la seguridad contra inyección SQL, el manejo de errores HTTP y la eficiencia asíncrona.

---

## Cuándo Utilizar Esta Skill
- Al revisar nuevos endpoints agregados o modificados en la capa `Web/`.
- Al auditar servicios de negocio (`Negocio/`) y repositorios de datos (`Datos/`).
- Al evaluar el cumplimiento de las directrices del proyecto (`AGENTS.md`).
- Para validar la seguridad en consultas SQL y manejo de secretos (`.env`).
- Al preparar pruebas de integración manuales con archivos `.http`.

---

## Pasos de Revisión (Protocolo en 5 Fases)

Sigue rigurosamente estos 5 pasos de revisión al auditar cualquier componente o funcionalidad de la REST API:

```
┌────────────────────────────────────────────────────────┐
│             PASOS DE REVISIÓN DE REST API              │
├────────────────────────────────────────────────────────┤
│ 1. Contrato HTTP y Endpoints (Capa Web)                │
│    └─ Verbos, URLs, Status Codes, MapGroup             │
│ 2. Lógica de Dominio y Validaciones (Capa Negocio)     │
│    └─ Reglas, Inyección de Dependencias, Excepciones   │
│ 3. Acceso a Datos y Seguridad SQL (Capa Datos)         │
│    └─ Cero concatenación, await using, async real      │
│ 4. Configuración, Secretos y Variables de Entorno      │
│    └─ ConnectionStrings, .env, .gitignore              │
│ 5. Validación con Pruebas HTTP (Pruebas Manuales)      │
│    └─ RestApi.http, Happy Path, Casos de Borde         │
└────────────────────────────────────────────────────────┘
```

---

### Paso 1: Verificación de Contrato y Enrutamiento HTTP (Capa `Web/`)
Evaluar la exposición del endpoint contra las mejores prácticas de diseño RESTful:

1. **Uso correcto de métodos HTTP (Verbos):**
   - `GET`: Para consultar uno o múltiples recursos (idempotente y seguro).
   - `POST`: Para crear nuevos recursos (no idempotente). Debe devolver `201 Created` con la cabecera `Location` o URI al nuevo recurso.
   - `PUT`: Para reemplazo/actualización completa del recurso.
   - `PATCH`: Para actualización parcial.
   - `DELETE`: Para eliminación o borrado lógico. Debe responder `204 No Content` si no retorna contenido.

2. **Diseño de Rutas (URIs):**
   - Agrupación por recursos mediante `app.MapGroup("/recursos")`.
   - Nombres en sustantivo, plural y minúsculas (ej. `/platos`, `/categorias`, `/mesas`).
   - Evitar verbos en las URLs (incorrecto: `/platos/crear`, `/platos/eliminar`).
   - Parámetros de ruta fuertemente tipados (ej. `/{id:int}`).

3. **Códigos de Estado HTTP y Respuestas Estandarizadas:**
   - `200 OK`: Consulta o actualización exitosa con payload.
   - `201 Created`: Creación exitosa con la entidad y la URL del recurso (`Results.Created($"/...", entidad)`).
   - `204 NoContent`: Operación exitosa sin cuerpo (`Results.NoContent()`).
   - `400 BadRequest`: Parámetros o cuerpo con formato inválido (`ArgumentException`).
   - `404 NotFound`: Recurso inexistente (`KeyNotFoundException`).
   - `409 Conflict`: Violación de unicidad o regla de estado (`InvalidOperationException`).
   - Delegación centralizada a manejadores de respuesta (ej. `Respuestas.Manejar(...)`) evitando fugas de stack trace o respuestas no estructuradas.

4. **Separación de Responsabilidades:**
   - La capa Web **no** debe contener lógica de negocio ni sentencias SQL. Solo mapea parámetros, invoca al servicio y retorna el resultado HTTP.

---

### Paso 2: Evaluación de Lógica de Negocio y Reglas de Dominio (Capa `Negocio/`)
Verificar la coherencia, validación y encapsulación de reglas:

1. **Desacoplamiento e Inyección de Dependencias (DI):**
   - Los servicios deben depender exclusivamente de interfaces de repositorio (ej. `IPlatoRepositorio`, `ICategoriaRepositorio`), nunca de implementaciones concretas.
   - Inyección moderna mediante constructores primarios de C# (ej. `public class PlatoServicio(IPlatoRepositorio repo)`).

2. **Validaciones Previas (Fail Fast):**
   - Validar obligatoriedad y longitud de campos (`string.IsNullOrWhiteSpace`, `Length > X`).
   - Validar rangos numéricos y precios (`precio > 0`).
   - Limpieza y normalización de entradas (`Trim()`).
   - Verificación de llaves foráneas o dependencias cruzadas antes de ejecutar operaciones en BD (ej. validar que la categoría exista antes de agregar un plato).

3. **Reglas de Borrado Lógico vs. Físico:**
   - Respetar políticas de negocio: entidades clave (como `Platos`) deben implementar borrado lógico (`Disponible = 0 / false`) y no eliminarse con `DELETE` físico.

4. **Manejo Semántico de Excepciones:**
   - Lanzar excepciones tipadas reconocibles por el manejador web:
     - `ArgumentException` para validaciones de datos del cliente (400).
     - `KeyNotFoundException` cuando el recurso a obtener, editar o eliminar no existe (404).
     - `InvalidOperationException` cuando se infrinja una regla de estado o unicidad (409).

---

### Paso 3: Auditoría de Acceso a Datos y Seguridad SQL (Capa `Datos/`)
Garantizar la integridad, seguridad y rendimiento en la interacción con la base de datos:

1. **Prevención Estricta de Inyección SQL (CRÍTICO):**
   - **PROHIBIDO** concatenar variables en cadenas SQL (ej. `$"SELECT ... WHERE Id = {id}"` es inadmisible).
   - Uso obligatorio de parámetros parametrizados (`SqlParameter` o `cmd.Parameters.AddWithValue("@id", id)`).

2. **Gestión de Recursos y Conexiones:**
   - Uso de `await using` en `SqlConnection`, `SqlCommand` y `SqlDataReader` para asegurar su liberación inmediata incluso ante excepciones.
   - Apertura explícita y asíncrona: `await cn.OpenAsync()`.

3. **Asincronía Real:**
   - Todos los métodos de repositorio deben ser asíncronos y terminar con el sufijo `Async` (ej. `ObtenerPorIdAsync`, `AgregarAsync`).
   - Uso de llamadas no bloqueantes: `ExecuteReaderAsync()`, `ExecuteNonQueryAsync()`, `ExecuteScalarAsync()`.

4. **Calidad y Eficiencia de Consultas SQL:**
   - Seleccionar columnas explícitas; evitar `SELECT *` descontrolado.
   - En consultas de lectura, incluir siempre el filtro de disponibilidad activa (`WHERE Disponible = 1`) cuando aplique borrado lógico.
   - Consultas con `JOIN` adecuadas para evitar el problema de N+1 consultas.

---

### Paso 4: Seguridad, Variables de Entorno y Configuración
Verificar que la configuración no comprometa secretos ni el entorno:

1. **Cadenas de Conexión:**
   - Obtener la cadena de conexión desde configuración (`config.GetConnectionString("Restaurante")` o variables de entorno `ConnectionStrings__Restaurante`).
   - Nunca incluir contraseñas o datos de conexión en duro (hardcoded) en código C# o en `appsettings.json`.

2. **Control de Versiones (`.gitignore`):**
   - Verificar que archivos sensibles (`.env`, `*.user`, `bin/`, `obj/`, carpetas temporales) estén ignorados y no se suban al repositorio.

---

### Paso 5: Validación con Pruebas HTTP (Pruebas Manuales)
Confirmar que el endpoint funciona según lo especificado:

1. **Actualización de `RestApi.http`:**
   - Cada nuevo endpoint debe contar con su correspondiente bloque de prueba en el archivo `.http`.
   - Probar casos de éxito (Happy Path):
     - `GET /recursos`
     - `POST /recursos` con payload válido -> esperar `201 Created`
     - `GET /recursos/{id}` -> esperar `200 OK`
     - `PUT /recursos/{id}` -> esperar `200 OK`
     - `DELETE /recursos/{id}` -> esperar `204 NoContent`
2. **Pruebas de Casos de Borde (Negative / Edge Cases):**
   - Enviar payload con datos inválidos o incompletos -> esperar `400 BadRequest`.
   - Consultar o modificar un ID inexistente -> esperar `404 NotFound`.
   - Violar reglas de negocio (ej. categoría inválida) -> esperar `400` o `409 Conflict`.

---

## Checklist Rápido de Verificación

Al emitir una revisión, utiliza esta lista de control:

| Criterio | Verificación | Estado |
| :--- | :--- | :---: |
| **Rutas & Verbos** | Verbo HTTP semántico y ruta RESTful en plural (`/recursos`) | [ ] |
| **Status Codes** | 200, 201 (con location), 204, 400, 404, 409 según corresponda | [ ] |
| **Separación Capas** | Web no tiene SQL; Negocio valida; Datos consulta | [ ] |
| **Inyección de Deps** | Servicios reciben interfaces (`IRepositorio`), no clases concretas | [ ] |
| **Seguridad SQL** | Parámetros obligatorios (`@param`). CERO concatenación | [ ] |
| **Recursos** | `await using` en conexiones, comandos y lectores | [ ] |
| **Asincronía** | Métodos con sufijo `Async` y llamadas asíncronas reales | [ ] |
| **Borrado Lógico** | Implementado con `Disponible = 0` donde la regla lo exige | [ ] |
| **Secretos** | Cadena de conexión desde `.env` / Configuration | [ ] |
| **Pruebas .http** | Endpoints documentados y probados en archivo `.http` | [ ] |

---

## Formato del Dictamen de Revisión

Al presentar los resultados de una revisión de REST API al usuario, estructura la respuesta con el siguiente formato:

````markdown
### 🔎 Resumen de la Revisión REST API
- **Módulo / Endpoint**: `<Ruta o Clase>`
- **Resultado Global**: `[APROBADO]` / `[REQUIERE CAMBIOS]` / `[RECHAZADO]`

---

### 📋 Hallazgos por Capa

#### 1. Capa Web (Endpoints)
- **Estado**: `[✓] Correcto` | `[⚠] Advertencia` | `[✗] Error`
- **Detalle**: `<Comentarios>`

#### 2. Capa de Negocio (Servicios y Validaciones)
- **Estado**: `[✓] Correcto` | `[⚠] Advertencia` | `[✗] Error`
- **Detalle**: `<Comentarios>`

#### 3. Capa de Datos (Repositorios y SQL)
- **Estado**: `[✓] Correcto` | `[⚠] Advertencia` | `[✗] Error`
- **Detalle**: `<Comentarios>`

---

### 🛠️ Correcciones Sugeridas / Código Refactorizado
```csharp
// Código refactorizado con las mejoras aplicadas
```

---

### 🧪 Pruebas HTTP Sugeridas (`RestApi.http`)
```http
### Prueba de Endpoint
GET {{host}}/recursos
Accept: application/json
```
````
