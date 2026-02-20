using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace TuProyecto.Services
{
    /// <summary>
    /// Cliente HTTP de muy bajo nivel para consumir la API oficial de PUBG.
    /// 
    /// Responsabilidades principales:
    /// - Construir peticiones HTTP con los headers requeridos (Authorization y Accept).
    /// - Obtener el JSON (string) desde la API y validar códigos de estado comunes.
    /// - Deserializar el JSON a un tipo <typeparamref name="T"/>.
    /// - Cachear ciertos resultados en memoria (MemoryCache) para evitar llamadas repetidas.
    /// 
    /// NOTA:
    /// - Esto no es un "SDK" completo; está pensado como una capa de infraestructura simple.
    /// - No contiene lógica de UI ni de negocio, solo comunicación HTTP.
    /// </summary>
    public sealed class PubgApiCliente
    {
        /// <summary>
        /// HttpClient es caro de crear/descartar. Por eso se recomienda reutilizarlo.
        /// 
        /// - Estático: una sola instancia para toda la app.
        /// - Evita problemas de agotamiento de sockets (muy típico si haces new HttpClient() por request).
        /// </summary>
        private static readonly HttpClient http = new HttpClient();

        /// <summary>
        /// API Key (token) de PUBG.
        /// Se envía como header: Authorization: Bearer {apiKey}
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// Caché en memoria del proceso.
        /// MemoryCache.Default es compartido por AppDomain.
        /// </summary>
        private readonly MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// Plataforma / shard.
        /// En PUBG la ruta suele depender del shard. Aquí usamos Steam.
        /// </summary>
        private const string Plataforma = "steam";

        /// <summary>
        /// Crea el cliente indicando la API Key.
        /// </summary>
        /// <param name="apiKey">Token de acceso. Debe venir sin saltos de línea ni espacios extra.</param>
        public PubgApiCliente(string apiKey)
        {
            // Guardar el token tal cual. Si contiene \r\n al final, se romperá el header Authorization.
            this.apiKey = (apiKey ?? "").Trim();

            if (string.IsNullOrWhiteSpace(this.apiKey))
            {
                throw new ArgumentException("API Key inválida.");
            }
        }

        /// <summary>
        /// Construye la petición GET con los headers requeridos por la API de PUBG.
        /// </summary>
        /// <param name="url">URL absoluta a consultar.</param>
        private HttpRequestMessage CrearRequest(string url)
        {
            // HttpRequestMessage es IDisposable (por eso luego la usamos dentro de using).
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            // PUBG requiere Bearer token.
            // TryAddWithoutValidation evita que el framework rechace valores por formato (útil si el token es largo).
            req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}");

            // Formato esperado por PUBG.
            req.Headers.TryAddWithoutValidation("Accept", "application/vnd.api+json");
            return req;
        }

        /// <summary>
        /// Ejecuta realmente la llamada HTTP y devuelve el body como texto JSON.
        /// Aquí se hace la validación de códigos de estado y mensajes de error.
        /// </summary>
        /// <param name="url">URL absoluta a consultar.</param>
        /// <param name="ct">Token de cancelación (por ejemplo si el usuario hace otra búsqueda).</param>
        private async Task<string> GetJsonAsync(string url, CancellationToken ct)
        {
            try
            {
                // "using" asegura liberar recursos nativos asociados a request/response.
                using (var req = CrearRequest(url))
                using (var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct))
                {
                    // Leer el body como string.
                    // Nota: si el body es grande, podrías usar streaming, pero aquí simplificamos.
                    string body = await resp.Content.ReadAsStringAsync();

                    // Manejo de errores más comunes con mensajes más entendibles.
                    if (resp.StatusCode == HttpStatusCode.Unauthorized || resp.StatusCode == HttpStatusCode.Forbidden)
                    {
                        // Normalmente significa API key inválida o sin permisos.
                        throw new Exception($"No autorizado. Revisa la API Key.\n\n{body}");
                    }

                    if ((int)resp.StatusCode >= 500)
                    {
                        // Fallo del servidor de PUBG.
                        throw new Exception($"Error del servidor de la API.\n\n{body}");
                    }

                    if (!resp.IsSuccessStatusCode)
                    {
                        // Cualquier otro 4xx (por ejemplo 400, 404, etc.)
                        throw new Exception($"Error HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\n\nURL:\n{url}\n\nRespuesta:\n{body}");
                    }

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        // Aunque sea 200 OK, un body vacío no sirve para deserializar.
                        throw new Exception("Respuesta vacía de la API");
                    }

                    return body;
                }
            }
            catch (HttpRequestException)
            {
                // Excepción típica de red (DNS, sin acceso a internet, etc.)
                throw new Exception("Sin conexión o error de red");
            }
        }

        /// <summary>
        /// Obtiene un endpoint, aplicando caché y deserializando a <typeparamref name="T"/>.
        /// 
        /// Flujo:
        /// 1) Comprueba si existe en caché usando <paramref name="claveCache"/>.
        /// 2) Si no existe: llama a la API, obtiene JSON y deserializa.
        /// 3) Guarda el resultado en caché durante <paramref name="duracion"/>.
        /// </summary>
        /// <typeparam name="T">Tipo a deserializar (DTO).</typeparam>
        /// <param name="url">URL absoluta a consultar.</param>
        /// <param name="claveCache">Clave única para ese tipo de consulta.</param>
        /// <param name="duracion">Tiempo a mantener el dato en caché.</param>
        /// <param name="ct">Token de cancelación.</param>
        public async Task<T> GetAsync<T>(string url, string claveCache, TimeSpan duracion, CancellationToken ct)
        {
            // 1) Intento rápido: caché.
            var objCache = cache.Get(claveCache);
            if (objCache is T cached)
            {
                return cached;
            }

            // 2) Llamada real.
            string json = await GetJsonAsync(url, ct);

            // 3) Deserializa JSON -> DTO.
            // Si el JSON no coincide con los DTOs, aquí podría salir null o lanzar excepción.
            var obj = JsonConvert.DeserializeObject<T>(json);

            // 4) Guardar en caché.
            cache.Set(claveCache, obj, DateTimeOffset.Now.Add(duracion));
            return obj;
        }

        /// <summary>
        /// Construye la URL para buscar un jugador por nombre.
        /// </summary>
        public string UrlPlayersPorNombre(string nombreJugador)
        {
            // Trim por seguridad: elimina espacios antes/después.
            string nombre = (nombreJugador ?? "").Trim();

            // EscapeDataString evita que caracteres especiales rompan la querystring.
            return $"https://api.pubg.com/shards/{Plataforma}/players?filter[playerNames]={Uri.EscapeDataString(nombre)}";
        }

        /// <summary>
        /// Construye la URL para obtener el detalle de un match por id.
        /// </summary>
        public string UrlMatch(string matchId)
        {
            return $"https://api.pubg.com/shards/{Plataforma}/matches/{Uri.EscapeDataString(matchId)}";
        }
    }
}