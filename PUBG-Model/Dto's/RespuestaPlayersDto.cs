using Newtonsoft.Json;
using System.Collections.Generic;

namespace TuProyecto.Models.Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para la respuesta del endpoint de jugadores:
    /// 
    /// GET /players?filter[playerNames]=...
    /// 
    /// La API de PUBG usa el formato JSON:API y suele devolver una propiedad "data"
    /// que es una lista de elementos.
    /// </summary>
    public sealed class RespuestaPlayersDto
    {
        /// <summary>
        /// Lista de jugadores devueltos por la API.
        /// 
        /// En JSON viene como:
        /// {
        ///   "data": [ { ... }, { ... } ]
        /// }
        /// </summary>
        [JsonProperty("data")]
        public List<PlayerDataDto> Data { get; set; }
    }

    /// <summary>
    /// Representa un elemento dentro de "data" para jugadores.
    /// 
    /// Normalmente incluye:
    /// - id (string)
    /// - attributes (datos del jugador)
    /// - relationships (referencias a matches, etc.)
    /// </summary>
    public sealed class PlayerDataDto
    {
        /// <summary>
        /// Identificador interno del jugador en PUBG.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Datos "propios" del jugador.
        /// En el JSON viene dentro de "attributes".
        /// </summary>
        [JsonProperty("attributes")]
        public PlayerAttributesDto Attributes { get; set; }

        /// <summary>
        /// Relaciones hacia otras entidades.
        /// Por ejemplo: matches.
        /// </summary>
        [JsonProperty("relationships")]
        public PlayerRelationshipsDto Relationships { get; set; }
    }

    /// <summary>
    /// Atributos del jugador.
    /// 
    /// Nota importante de naming:
    /// - En JSON se llama "name".
    /// - En C# lo llamamos "Nombre" para tener nombres en español.
    /// - El atributo [JsonProperty] es el que hace el mapeo.
    /// </summary>
    public sealed class PlayerAttributesDto
    {
        [JsonProperty("name")]
        public string Nombre { get; set; }
    }

    /// <summary>
    /// Bloque de relaciones del jugador.
    /// </summary>
    public sealed class PlayerRelationshipsDto
    {
        /// <summary>
        /// Relación hacia los matches del jugador.
        /// 
        /// En JSON suele verse como:
        /// "matches": { "data": [ {"id": "...", "type": "match"}, ... ] }
        /// </summary>
        [JsonProperty("matches")]
        public MatchesRelacionDto Matches { get; set; }
    }

    /// <summary>
    /// Representa la relación de matches y lista sus identificadores.
    /// </summary>
    public sealed class MatchesRelacionDto
    {
        /// <summary>
        /// Lista de referencias (id + type), no es el match completo.
        /// Para el detalle se llama luego al endpoint /matches/{id}.
        /// </summary>
        [JsonProperty("data")]
        public List<RelacionIdDto> Data { get; set; }
    }

    /// <summary>
    /// Referencia (relación) hacia otra entidad.
    /// 
    /// En JSON:API es común que las relaciones vengan solo con:
    /// - id
    /// - type
    /// </summary>
    public sealed class RelacionIdDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Tipo de entidad (por ejemplo "match").
        /// En C# lo llamamos "Tipo".
        /// </summary>
        [JsonProperty("type")]
        public string Tipo { get; set; }
    }
}