using Newtonsoft.Json;

namespace TuProyecto.Models.Dtos
{
    /// <summary>
    /// DTO para la respuesta del endpoint de match:
    /// 
    /// GET /matches/{matchId}
    /// 
    /// En JSON:API la entidad principal viene en "data".
    /// Este DTO solo modela lo que la UI está usando ahora (id + attributes).
    /// </summary>
    public sealed class RespuestaMatchDto
    {
        /// <summary>
        /// Entidad principal del match.
        /// </summary>
        [JsonProperty("data")]
        public MatchDataDto Data { get; set; }
    }

    /// <summary>
    /// Representa el nodo "data" del match.
    /// 
    /// Ejemplo simplificado:
    /// {
    ///   "data": {
    ///     "id": "...",
    ///     "attributes": { ... }
    ///   }
    /// }
    /// </summary>
    public sealed class MatchDataDto
    {
        /// <summary>
        /// Id del match (string).
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Atributos del match (mapa, modo, duración...).
        /// </summary>
        [JsonProperty("attributes")]
        public MatchAttributesDto Attributes { get; set; }
    }

    /// <summary>
    /// Atributos del match.
    /// 
    /// Nota de naming:
    /// - PUBG entrega nombres en inglés (createdAt, duration, gameMode, mapName)
    /// - En C# los renombramos a español (CreadoEn, DuracionSegundos, Modo, Mapa)
    /// - [JsonProperty] es el puente entre ambos.
    /// 
    /// Nota de tipos:
    /// - "duration" es numérico (segundos).
    /// - "createdAt" normalmente viene como string ISO-8601; aquí lo guardamos como string
    ///   para evitar problemas de parseo en UI. Si quisieras, podrías cambiarlo a DateTime.
    /// </summary>
    public sealed class MatchAttributesDto
    {
        /// <summary>
        /// Fecha/hora de inicio del match.
        /// JSON: "createdAt".
        /// 
        /// Se deja como string (normalmente ISO-8601) por simplicidad.
        /// </summary>
        [JsonProperty("createdAt")]
        public string CreadoEn { get; set; }

        /// <summary>
        /// Duración del match en segundos.
        /// JSON: "duration".
        /// </summary>
        [JsonProperty("duration")]
        public int DuracionSegundos { get; set; }

        /// <summary>
        /// Modo de juego.
        /// JSON: "gameMode".
        /// </summary>
        [JsonProperty("gameMode")]
        public string Modo { get; set; }

        /// <summary>
        /// Nombre del mapa.
        /// JSON: "mapName".
        /// </summary>
        [JsonProperty("mapName")]
        public string Mapa { get; set; }
    }
}