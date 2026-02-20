using System.Windows;
using System.Windows.Controls;

namespace PUBG_Views.Controls
{
    /// <summary>
    /// Control de UI simple que muestra información básica del jugador.
    /// 
    /// Se usa desde `MainWindow` llamando a `SetJugador(...)`.
    /// No consulta datos ni llama a la API: solo pinta texto.
    /// </summary>
    public partial class PlayerCardControl : UserControl
    {
        public PlayerCardControl()
        {
            // Carga el XAML asociado y crea los elementos (TxtNombre, TxtId...).
            InitializeComponent();
        }

        /// <summary>
        /// Actualiza el card con los datos del jugador.
        /// </summary>
        /// <param name="nombre">Nombre visible (puede venir null).</param>
        /// <param name="id">Id interno de PUBG (puede venir null).</param>
        public void SetJugador(string nombre, string id)
        {
            // Si el nombre está vacío o null, mostramos un placeholder.
            TxtNombre.Text = string.IsNullOrWhiteSpace(nombre) ? "-" : nombre;

            // El id si está vacío preferimos ocultarlo (string.Empty) en vez de "-".
            TxtId.Text = string.IsNullOrWhiteSpace(id) ? string.Empty : id;
        }
    }
}
