using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NexusVideojuegoEngine.Data;
using NexusVideojuegoEngine.Models;

namespace NexusVideojuegoEngine
{
    public partial class FormBovedaMemoria : Form
    {
        private readonly GestorBD _gestorBD = new GestorBD();
        private readonly int _idJugador;
        private readonly string _nombreUsuario;
        private readonly string _codigoRig;

        // Propiedad que devolverá la partida lista para jugar al FormMenu
        public PartidaCompletaDTO? PartidaSeleccionada { get; private set; }

        // Controles de interfaz
        private DataGridView dgvPartidas = null!;
        private GroupBox grpNuevaRanura = null!;
        private TextBox txtNombreRanura = null!;
        private Button btnCrearRanura = null!;
        private Button btnCargarRanura = null!;
        private Button btnEliminarRanura = null!;
        private Button btnVolver = null!;
        private Label lblEstadoBoveda = null!;

        public FormBovedaMemoria(int idJugador, string nombreUsuario, string codigoRig)
        {
            InitializeComponent();
            _idJugador = idJugador;
            _nombreUsuario = nombreUsuario;
            _codigoRig = codigoRig;

            ConstruirInterfazBoveda();
            CargarTablaPartidas();
        }

        private void ConstruirInterfazBoveda()
        {
            this.Controls.Clear();
            this.Text = $"DEAD SIGNAL // BÓVEDA DE MEMORIA SQL — Operador: {_nombreUsuario} [{_codigoRig}]";
            this.Size = new Size(920, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(12, 18, 32);

            Label lblTitulo = new Label
            {
                Text = "BÓVEDA DE MEMORIA RELACIONAL (DeadSignal_DB)",
                Font = new Font("Consolas", 14.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 182, 212),
                Location = new Point(25, 18),
                AutoSize = true
            };

            Label lblSubtitulo = new Label
            {
                Text = $"REGISTROS VINCULADOS AL OPERADOR #{_idJugador}: {_nombreUsuario.ToUpper()} [{_codigoRig}]",
                Font = new Font("Consolas", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(27, 46),
                AutoSize = true
            };

            // =================================================================
            // SECCIÓN SUPERIOR: CREAR NUEVA RANURA DE DESPLIEGUE
            // =================================================================
            grpNuevaRanura = new GroupBox
            {
                Text = " [ NUEVO REGISTRO DE DESPLIEGUE ] ",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(25, 78),
                Size = new Size(855, 78)
            };

            Label lblRanura = new Label
            {
                Text = "NOMBRE DE RANURA:",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(18, 34),
                AutoSize = true
            };

            txtNombreRanura = new TextBox
            {
                Location = new Point(175, 30),
                Size = new Size(360, 28),
                Font = new Font("Consolas", 10.5f),
                BackColor = Color.FromArgb(23, 33, 54),
                ForeColor = Color.FromArgb(52, 211, 153),
                BorderStyle = BorderStyle.FixedSingle,
                Text = $"Cubierta09_{DateTime.Now:ddMM_HHmm}"
            };

            btnCrearRanura = new Button
            {
                Text = "+ CREAR Y DESPLEGAR RANURA",
                Location = new Point(555, 26),
                Size = new Size(280, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCrearRanura.FlatAppearance.BorderSize = 0;
            btnCrearRanura.Click += BtnCrearRanura_Click;

            grpNuevaRanura.Controls.Add(lblRanura);
            grpNuevaRanura.Controls.Add(txtNombreRanura);
            grpNuevaRanura.Controls.Add(btnCrearRanura);

            // =================================================================
            // SECCIÓN CENTRAL: TABLA DE PARTIDAS GUARDADAS (DataGridView)
            // =================================================================
            dgvPartidas = new DataGridView
            {
                Location = new Point(25, 172),
                Size = new Size(855, 265),
                BackgroundColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.FromArgb(51, 65, 85),
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Consolas", 9.5f)
            };

            dgvPartidas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvPartidas.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(245, 158, 11);
            dgvPartidas.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 9.5f, FontStyle.Bold);
            dgvPartidas.ColumnHeadersHeight = 34;

            dgvPartidas.DefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvPartidas.DefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 240);
            dgvPartidas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 99, 235);
            dgvPartidas.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvPartidas.RowTemplate.Height = 30;

            // =================================================================
            // SECCIÓN INFERIOR: BOTONES DE ACCIÓN Y ESTADO
            // =================================================================
            btnCargarRanura = new Button
            {
                Text = "> CARGAR RANURA SELECCIONADA",
                Location = new Point(25, 455),
                Size = new Size(280, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCargarRanura.FlatAppearance.BorderSize = 0;
            btnCargarRanura.Click += BtnCargarRanura_Click;

            btnEliminarRanura = new Button
            {
                Text = "X PURGAR REGISTRO",
                Location = new Point(320, 455),
                Size = new Size(200, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(153, 27, 27),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEliminarRanura.FlatAppearance.BorderSize = 0;
            btnEliminarRanura.Click += BtnEliminarRanura_Click;

            btnVolver = new Button
            {
                Text = "VOLVER AL MENÚ",
                Location = new Point(710, 455),
                Size = new Size(170, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);
            btnVolver.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            lblEstadoBoveda = new Label
            {
                Text = "Seleccione una ranura para restaurar constantes vitales o cree una nueva.",
                Font = new Font("Consolas", 8.5f),
                ForeColor = Color.FromArgb(52, 211, 153),
                Location = new Point(25, 503),
                AutoSize = true
            };

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblSubtitulo);
            this.Controls.Add(grpNuevaRanura);
            this.Controls.Add(dgvPartidas);
            this.Controls.Add(btnCargarRanura);
            this.Controls.Add(btnEliminarRanura);
            this.Controls.Add(btnVolver);
            this.Controls.Add(lblEstadoBoveda);
        }

        private void CargarTablaPartidas()
        {
            try
            {
                DataSet ds = _gestorBD.ObtenerPartidasDataSet(_idJugador);
                dgvPartidas.DataSource = ds.Tables["PartidasGuardadas"];

                if (dgvPartidas.Columns.Contains("IdPartida"))
                {
                    dgvPartidas.Columns["IdPartida"]!.Visible = false;
                }

                int total = dgvPartidas.Rows.Count;
                lblEstadoBoveda.ForeColor = Color.FromArgb(52, 211, 153);
                lblEstadoBoveda.Text = total > 0
                    ? $"BÓVEDA SINCRONIZADA: {total} ranura(s) encontrada(s) en SQL Server."
                    : "BÓVEDA VACÍA: Cree una nueva ranura de despliegue arriba para iniciar.";

                btnCargarRanura.Enabled = total > 0;
                btnEliminarRanura.Enabled = total > 0;
            }
            catch (Exception ex)
            {
                lblEstadoBoveda.ForeColor = Color.FromArgb(248, 113, 113);
                lblEstadoBoveda.Text = "Error al leer registros desde SQL Server.";
                MessageBox.Show($"Error al consultar partidas:\n{ex.Message}", "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCrearRanura_Click(object? sender, EventArgs e)
        {
            string nombreRanura = txtNombreRanura.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreRanura))
            {
                MessageBox.Show("Ingrese un nombre identificador para la ranura de guardado.", "Nombre requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreRanura.Focus();
                return;
            }

            try
            {
                EstadoCadeteDTO estadoInicial = new EstadoCadeteDTO
                {
                    VidaActual = 100,
                    VidaMax = 100,
                    ExperienciaXP = 0,
                    Nivel = 1,
                    PosicionX = 1,
                    PosicionY = 1,
                    CargasEMP = 2
                };

                List<EntidadMapaDTO> entidadesIniciales = new List<EntidadMapaDTO>();
                int nuevoIdPartida = _gestorBD.CrearNuevaPartida(_idJugador, nombreRanura, estadoInicial, entidadesIniciales);

                PartidaSeleccionada = _gestorBD.CargarPartidaCompleta(nuevoIdPartida);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo crear la ranura en SQL Server:\n{ex.Message}", "Error de Transacción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCargarRanura_Click(object? sender, EventArgs e)
        {
            if (dgvPartidas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una fila de la tabla para cargar la partida.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int idPartida = Convert.ToInt32(dgvPartidas.SelectedRows[0].Cells["IdPartida"].Value);
                PartidaCompletaDTO? partida = _gestorBD.CargarPartidaCompleta(idPartida);

                if (partida == null)
                {
                    MessageBox.Show("No se encontraron los datos completos de la partida seleccionada.", "Registro incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PartidaSeleccionada = partida;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restaurar la partida desde SQL Server:\n{ex.Message}", "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarRanura_Click(object? sender, EventArgs e)
        {
            if (dgvPartidas.SelectedRows.Count == 0) return;

            int idPartida = Convert.ToInt32(dgvPartidas.SelectedRows[0].Cells["IdPartida"].Value);
            string nombreRanura = dgvPartidas.SelectedRows[0].Cells["Ranura"].Value?.ToString() ?? "Seleccionada";

            DialogResult confirmar = MessageBox.Show(
                $"¿Está seguro de purgar permanentemente la ranura '{nombreRanura}'?\nSe borrarán también las constantes vitales, entidades y bitácora asociadas.",
                "Confirmar Purga en SQL Server",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmar != DialogResult.Yes) return;

            try
            {
                if (_gestorBD.EliminarPartida(idPartida))
                {
                    CargarTablaPartidas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar el registro:\n{ex.Message}", "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}