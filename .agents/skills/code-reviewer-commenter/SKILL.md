---
name: code-reviewer-commenter
description: >-
  Use this skill when the user asks to explain code line-by-line, add meaningful comments to code, evaluate whether code is right or wrong, identify bugs or anti-patterns, or suggest improvements and corrections.
---

# Code Reviewer & Line-by-Line Commenter Skill

Esta habilidad guía al asistente para analizar cualquier fragmento de código o archivo, explicar minuciosamente qué hace cada línea o bloque, evaluar su corrección (si está bien, mal o mejorable) y generar comentarios claros y profesionales.

---

## Metodología de Análisis

Cuando el usuario solicite revisar un archivo o fragmento de código, sigue este proceso de 4 fases:

### 1. Contextualización
- Identifica el lenguaje, framework, arquitectura y dependencias (por ejemplo, C# 14, .NET 10, Minimal APIs, ADO.NET).
- Consulta las reglas de arquitectura y convenciones locales (ej. `AGENTS.md`, `.editorconfig`).

### 2. Desglose y Explicación Línea por Línea
Para cada línea o bloque lógico del código, presenta un análisis estructurado con los siguientes elementos:

1. **Línea / Bloque**: El código exacto analizado.
2. **¿Qué hace?**: Explicación técnica clara en lenguaje accesible.
3. **Diagnóstico**:
   - `[✓] Correcto`: Cumple buenas prácticas, no tiene errores ni fugas.
   - `[⚠] Mejorable`: Funciona, pero tiene oportunidades de optimización, legibilidad o estándares.
   - `[✗] Error / Riesgo`: Error lógico, bug de concurrencia, riesgo de seguridad (ej. inyección SQL), violación de arquitectura o excepción no controlada.
4. **¿Por qué? y ¿Qué cambiar?**: Justificación detallada si requiere cambio.

### 3. Criterios de Evaluación Clave
- **Seguridad**: Prevención de inyección SQL, manejo seguro de secretos (`.env`), validación de entradas.
- **Rendimiento y Recursos**: Uso correcto de `async/await`, liberación de conexiones (`await using`), consultas eficientes.
- **Arquitectura y Limpieza**: Respeto a la separación de capas (Entidades, Datos, Negocio, Web), desacoplamiento con interfaces, manejo de excepciones tipadas.
- **Estándares del Lenguaje**: Naming conventions, nullability (`?`, `!`), tipos modernos (primary constructors, pattern matching).

### 4. Generación de Código Comentado y Corregido
- Proveer el código final refactorizado.
- Insertar comentarios útiles que expliquen el **propósito** y el **por qué** de las decisiones críticas, evitando comentarios obvios o ruido visual (por ejemplo, evitar `// suma 1 a i`).

---

## Formato Estándar de Respuesta

Cuando actives esta habilidad, estructura tu respuesta de la siguiente forma:

````markdown
### 📋 Resumen del Componente
- **Archivo/Elemento**: `<nombre_archivo>`
- **Responsabilidad principal**: `<qué función cumple en el sistema>`

---

### 🔍 Análisis Línea por Línea

| Línea / Bloque | ¿Qué hace? | Estado | Diagnóstico y Sugerencia |
| :--- | :--- | :---: | :--- |
| `<código>` | `<explicación>` | `[✓]` / `[⚠]` / `[✗]` | `<justificación y qué cambiar si aplica>` |

---

### 🛠️ Código Corregido y Comentado

```<lenguaje>
// Código con comentarios claros y mejoras aplicadas
```

### 💡 Puntos Clave de Mejora
- Resumen de los cambios principales aplicados y beneficios.
````

---

## Recursos y Ejemplos

- [Ejemplo de Revisión](./examples/ejemplo-revision.md): Ejemplo práctico de análisis línea por línea sobre código C# .NET.
