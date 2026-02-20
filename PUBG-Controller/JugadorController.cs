using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TuProyecto.Models.Dtos;
using TuProyecto.Services;

namespace TuProyecto.Controllers
{
    /// <summary>
    /// Capa "Controller" (lógica de aplicación) encargada de:
    /// - Validar entradas simples (por ejemplo nombre vacío).
    /// - Llamar a la capa de servicios (PubgApiCliente).
    /// - Traducir respuestas a objetos que consumirá la UI.
    /// 
    /// No debería contener código de UI (MessageBox, controles WPF...),
    /// ni código HTTP de bajo nivel (eso vive en PubgApiCliente).
    /// </summary>
    public sealed class JugadorController
    {
        /// <summary>
        /// Servicio que sabe hablar con la API de PUBG.
        /// </summary>
        private readonly PubgApiCliente api;

        /// <summary>
        /// Inyección del servicio API.
        /// Esto facilita testear, reutilizar y separar responsabilidades.
        /// </summary>
        public JugadorController(PubgApiCliente api)
        {
            this.api = api;
        }

        /// <summary>
        /// Busca un jugador por nombre.
        /// </summary>
        /// <param name="nombre">Nombre del jugador (tal cual lo escribe el usuario).</param>
        /// <param name="ct">
        /// Token de cancelación.
        /// Útil en UI: si el usuario hace otra búsqueda, se cancela la anterior.
        /// </param>
        /// <returns>El primer jugador devuelto por la API (DTO).</returns>
        public async Task<PlayerDataDto> BuscarJugadorAsync(string nombre, CancellationToken ct)
        {
            // Validación básica para evitar requests inútiles.
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("Escribe un nombre de jugador");
            }

            // Limpieza de espacios.
            string limpio = nombre.Trim();

            // Construcción de URL (no se hace hardcode aquí; lo hace el servicio).
            var url = api.UrlPlayersPorNombre(limpio);

            // Llamada a la API con caché.
            // claveCache: identifica unívocamente esta consulta.
            // duracion: evita spamear la API si el usuario busca lo mismo seguido.
            var resp = await api.GetAsync<RespuestaPlayersDto>(
                url,
                claveCache: $"player_{limpio.ToLower()}",
                duracion: TimeSpan.FromMinutes(5),
                ct: ct
            );

            // La API devuelve una lista (data[]). Tomamos el primero.
            var jugador = resp?.Data?.FirstOrDefault();
            if (jugador == null)
            {
                // Si no hay resultados, informamos a la UI.
                throw new Exception("Jugador no encontrado en steam");
            }

            return jugador;
        }

        /// <summary>
        /// Obtiene el detalle de un match a partir de su id.
        /// </summary>
        /// <param name="matchId">Id del match (viene de la lista del jugador).</param>
        /// <param name="ct">Token de cancelación.</param>
        public async Task<RespuestaMatchDto> ObtenerMatchAsync(string matchId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(matchId))
            {
                throw new Exception("MatchId inválido");
            }

            var url = api.UrlMatch(matchId);

            // Guardamos el resultado en caché porque varios clics sobre el mismo match
            // no deberían disparar nuevas llamadas.
            return await api.GetAsync<RespuestaMatchDto>(
                url,
                claveCache: $"match_{matchId}",
                duracion: TimeSpan.FromMinutes(10),
                ct: ct
            );
        }
    }
}