using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TuProyecto.Controllers;
using TuProyecto.Services;

namespace TuProyecto.Views
{
    /// <summary>
    /// Ventana principal (WPF).
    /// 
    /// Aquí vive la lógica de interacción de UI:
    /// - Leer lo que escribe el usuario.
    /// - Llamar al controlador.
    /// - Pintar resultados en los controles.
    /// - Gestionar estados: cargando, error, deshabilitar botones, etc.
    /// 
    /// Nota: este ejemplo usa code-behind (eventos). No es MVVM puro, pero es válido
    /// para un proyecto pequeño o de aprendizaje.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Orquestador de llamadas/validaciones.
        /// La ventana no debería construir URLs ni hacer HTTP directamente.
        /// </summary>
        private readonly JugadorController controlador;

        /// <summary>
        /// Fuente de cancelación para las operaciones async.
        /// 
        /// Estrategia:
        /// - Si el usuario hace una nueva búsqueda o selecciona otro match,
        ///   cancelamos la operación anterior para que no "pise" la UI.
        /// </summary>
        private CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();

            // Crear el cliente de la API.
            // IMPORTANTE: el token debe estar limpio (sin \r\n al final) o el header Authorization se rompe.
            var api = new PubgApiCliente(apiKey: "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIwMDg5MDA1MC1kNzhlLTAxM2UtNTIwZC0wYWQ2YjhkZjQ2OGYiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNzY4ODQ1MzQ1LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6ImNvbmV4aW9uLWFwaSJ9.nufXY8_Wqm5Aft874cOOxLHPZPYuhQkk4TSUWJmuc3s");

            // Inyección manual del servicio en el controller.
            controlador = new JugadorController(api);

            // Estado inicial de la UI (sin jugador y sin match).
            PlayerCard.SetJugador(null, null);
            MatchDetail.SetDetalle(null);
        }

        /// <summary>
        /// Evento Click del botón Buscar.
        /// 
        /// Flujo:
        /// 1) Cancela cualquier operación anterior.
        /// 2) Valida el nombre.
        /// 3) Deshabilita UI y pone estado "Buscando".
        /// 4) Llama al controller para obtener el jugador.
        /// 5) Pinta el jugador y sus matches.
        /// </summary>
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Si había una petición anterior en curso, la cancelamos.
            cts?.Cancel();
            cts = new CancellationTokenSource();

            try
            {
                // Leer lo que escribió el usuario.
                // Trim: evita que espacios al principio/final rompan la búsqueda.
                var nombre = (TxtNombre.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    TxtEstado.Text = "Escribe un nombre de jugador";
                    return;
                }

                // Feedback: bloqueamos el botón para que no spamee requests.
                BtnBuscar.IsEnabled = false;

                // Limpieza visual previa.
                ListaMatches.ItemsSource = null;
                PlayerCard.SetJugador(null, null);
                MatchDetail.SetDetalle(null);
                TxtEstado.Text = "Buscando jugador...";

                // Llamada asíncrona.
                // Si el usuario vuelve a buscar antes de que termine, el token cts.Token cancelará.
                var jugador = await controlador.BuscarJugadorAsync(nombre, cts.Token);

                // Mostrar datos básicos.
                PlayerCard.SetJugador(jugador.Attributes?.Nombre, jugador.Id);

                // De la respuesta del jugador, sacamos ids de matches.
                // Take(25): limitamos el listado para no llenar la UI.
                var ids = jugador.Relationships?.Matches?.Data?
                    .Select(x => x.Id)
                    .Take(25)
                    .ToList();

                // Asignar al ListBox. En este caso es una lista de strings (ids).
                ListaMatches.ItemsSource = ids;

                TxtEstado.Text = $"Jugador cargado. Matches: {ids?.Count ?? 0}";
            }
            catch (OperationCanceledException)
            {
                // Se lanza cuando el CancellationToken se cancela.
                TxtEstado.Text = "Operación cancelada";
            }
            catch (Exception ex)
            {
                // Errores de validación, de red, HTTP, etc.
                TxtEstado.Text = ex.Message;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Pase lo que pase, reactivamos el botón.
                BtnBuscar.IsEnabled = true;
            }
        }

        /// <summary>
        /// Evento cuando cambias la selección del ListBox de matches.
        /// 
        /// Se ejecuta también si se asigna ItemsSource o se limpia,
        /// por eso primero comprobamos SelectedItem != null.
        /// </summary>
        private async void ListaMatches_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListaMatches.SelectedItem == null)
            {
                return;
            }

            // Como ItemsSource es una lista de strings, ToString() nos da el id.
            string matchId = ListaMatches.SelectedItem.ToString();

            // Cancelar posible request anterior (por ejemplo si el usuario hace clic rápido en varios matches).
            cts?.Cancel();
            cts = new CancellationTokenSource();

            try
            {
                TxtEstado.Text = "Cargando match...";

                // Poner el panel de detalle en modo "loading".
                MatchDetail.SetCargando();

                // Obtener detalle del match.
                var match = await controlador.ObtenerMatchAsync(matchId, cts.Token);

                // Pintar el detalle.
                MatchDetail.SetDetalle(match);

                TxtEstado.Text = "Match cargado";
            }
            catch (OperationCanceledException)
            {
                TxtEstado.Text = "Operación cancelada";
            }
            catch (Exception ex)
            {
                TxtEstado.Text = ex.Message;
                MessageBox.Show(ex.Message);
            }
        }
    }
}