using System;
using System.Windows.Controls;
using TuProyecto.Models.Dtos;

namespace PUBG_Views.Controls
{
    /// <summary>
    /// Control de UI que muestra el resumen de un match.
    /// 
    /// Estados típicos:
    /// - "Vacío": aún no se seleccionó ningún match.
    /// - "Cargando": se está consultando el match en la API.
    /// - "Detalle": ya tenemos DTO y lo pintamos.
    /// </summary>
    public partial class MatchDetailControl : UserControl
    {
        public MatchDetailControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pone el control en modo "loading".
        /// Se llama antes de iniciar la petición async.
        /// </summary>
        public void SetCargando()
        {
            TxtStatus.Text = "Cargando...";

            // Reseteamos campos para que no se vea el detalle anterior
            // mientras el nuevo match está cargando.
            TxtMatchId.Text = "-";
            TxtMap.Text = "-";
            TxtMode.Text = "-";
            TxtDuration.Text = "-";
            TxtCreated.Text = "-";
        }

        /// <summary>
        /// Pinta el DTO del match en pantalla.
        /// </summary>
        /// <param name="match">
        /// DTO deserializado desde la API.
        /// Si viene null, dejamos el control en estado "vacío".
        /// </param>
        public void SetDetalle(RespuestaMatchDto match)
        {
            // Si no hay match, mostramos mensaje "Selecciona un match".
            if (match?.Data == null)
            {
                TxtStatus.Text = "Selecciona un match";
                TxtMatchId.Text = "-";
                TxtMap.Text = "-";
                TxtMode.Text = "-";
                TxtDuration.Text = "-";
                TxtCreated.Text = "-";
                return;
            }

            // Helper: attributes puede venir null si la API cambia o la deserialización falla.
            var a = match.Data.Attributes;

            TxtStatus.Text = string.Empty;

            // Id del match.
            TxtMatchId.Text = match.Data.Id ?? "-";

            // Nombre del mapa y modo.
            // En tus DTOs se llaman Mapa y Modo (por eso no usamos MapName/GameMode).
            TxtMap.Text = string.IsNullOrWhiteSpace(a?.Mapa) ? "-" : a.Mapa;
            TxtMode.Text = string.IsNullOrWhiteSpace(a?.Modo) ? "-" : a.Modo;

            // Duración viene como segundos (int). Lo convertimos a hh:mm:ss para hacerlo legible.
            if (a != null && a.DuracionSegundos > 0)
            {
                TxtDuration.Text = TimeSpan.FromSeconds(a.DuracionSegundos).ToString(@"hh\:mm\:ss");
            }
            else
            {
                TxtDuration.Text = "-";
            }

            // createdAt en el DTO está como string (CreadoEn).
            // Podríamos parsear a DateTime, pero aquí lo mostramos tal cual para evitar errores de formato.
            TxtCreated.Text = string.IsNullOrWhiteSpace(a?.CreadoEn) ? "-" : a.CreadoEn;
        }
    }
}
