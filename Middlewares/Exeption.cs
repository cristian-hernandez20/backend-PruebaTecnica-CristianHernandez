using Newtonsoft.Json;
using Npgsql;

namespace Middlewares {
    public class ExceptionHandlingMiddleware(RequestDelegate next) {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context) {
            try {
                await _next(context);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresEx) {
                await HandleForeignKeyConstraintExceptionAsync(context, postgresEx);
            }
            catch (Exception ex) {
                await HandleGlobalExceptionAsync(context, ex);
            }
        }

        private static Task HandleForeignKeyConstraintExceptionAsync(HttpContext context, PostgresException ex) {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            string message = ex.SqlState switch {
                "23503" => $"No se puede eliminar el registro porque está siendo utilizado en otros registros.",
                "23505" => $"Violación de restricción de unicidad: ya existe un registro con la misma clave única.",
                "42703" => $"Error de base de datos: la estructura de la base de datos es incorrecta.",
                "42P01" => $"Error de base de datos: no se encuentra la tabla especificada.",
                "42P10" => $"Error de base de datos: la columna especificada no existe en la tabla.",
                "42P22" => $"Error de base de datos: la restricción de verificación falló.",
                "42P29" => $"Error de base de datos: el operador especificado no existe.",
                "42P91" => $"Error de base de datos: la restricción de llave foránea especificada no existe.",
                "42P99" => $"Error de base de datos: la sintaxis de la sentencia SQL es incorrecta o no es válida.",
                "XX000" => $"Error de base de datos desconocido.",
                _ => "Error de base de datos desconocido.",
            };

            var response = new {
                message,
                success = false,
                error = $"PostgresException -> {ex}",
                type = "error"
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private static Task HandleGlobalExceptionAsync(HttpContext context, Exception ex) {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new {
                success = false,
                message = ex.Message,
                error = $"Exception -> {ex}",
                type = "info"
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
