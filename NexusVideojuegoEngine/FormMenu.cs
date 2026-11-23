using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NexusVideojuegoEngine.Data;
using NexusVideojuegoEngine.Models;

namespace NexusVideojuegoEngine
{
    public partial class FormMenu : Form
    {
        private readonly GestorBD _gestorBD = new GestorBD();

        private GroupBox grpRigIdentificacion = null!;
        private Label lblRigUsuario = null!;
        private TextBox txtRigUsuario = null!;
        private Label lblRigCodigo = null!;
        private TextBox txtRigCodigo = null!;
        private Button btnRigConectar = null!;
        private Label lblRigEstado = null!;

        private GroupBox grpTermOperaciones = null!;
        private Button btnTermNarrativa = null!;
        private Button btnTermNuevaPartida = null!;
        private Button btnTermPartidasSQL = null!;
        private Button btnTermSalir = null!;

        private int idJugadorActivo = 0;
        private string cadeteActivo = string.Empty;
        private string codigoRigActivo = string.Empty;
        private bool sesionIniciada = false;

        public FormMenu()
        {
            InitializeComponent();
            ConstruirTerminalAcceso();
        }

        private void ConstruirTerminalAcceso()
        {
            this.Controls.Clear();

            this.Text = "DEAD SIGNAL: PROTOCOLO RIG // TERMINAL DE MANDO (v7.0)";
            this.Size = new Size(840, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 18, 32);

            Label lblHeader = new Label
            {
                Text = "DEAD SIGNAL: PROTOCOLO RIG  |  CUBIERTA ORBITAL 09",
                Font = new Font("Consolas", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(28, 22),
                AutoSize = true
            };

            Label lblSub = new Label
            {
                Text = "OPERACIÓN MEMORIA — SINCRONICE SU TARJETA RIG CON LA BÓVEDA SQL SERVER",
                Font = new Font("Consolas", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(30, 52),
                AutoSize = true
            };

            grpRigIdentificacion = new GroupBox
            {
                Text = " [ 01. AUTENTICACIÓN DE TRAJE RIG ] ",
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 182, 212),
                Location = new Point(28, 92),
                Size = new Size(365, 360)
            };

            lblRigUsuario = new Label
            {
                Text = "ALIAS DEL OPERADOR:",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(22, 42),
                AutoSize = true
            };

            txtRigUsuario = new TextBox
            {
                Location = new Point(22, 66),
                Size = new Size(315, 28),
                Font = new Font("Consolas", 11f),
                BackColor = Color.FromArgb(23, 33, 54),
                ForeColor = Color.FromArgb(52, 211, 153),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblRigCodigo = new Label
            {
                Text = "CÓDIGO DE UNIDAD RIG (Ej. RIG-09):",
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(22, 115),
                AutoSize = true
            };

            txtRigCodigo = new TextBox
            {
                Location = new Point(22, 139),
                Size = new Size(315, 28),
                Font = new Font("Consolas", 11f),
                BackColor = Color.FromArgb(23, 33, 54),
                ForeColor = Color.FromArgb(52, 211, 153),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "RIG-09"
            };

            btnRigConectar = new Button
            {
                Text = "SINCRONIZAR TARJETA RIG (SQL)",
                Location = new Point(22, 192),
                Size = new Size(315, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRigConectar.FlatAppearance.BorderSize = 0;
            btnRigConectar.Click += BtnRigConectar_Click;

            lblRigEstado = new Label
            {
                Text = "ESTADO SQL: DESCONECTADO\nIngrese su alias para validar en DeadSignal_DB.",
                Font = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(248, 113, 113),
                Location = new Point(22, 252),
                Size = new Size(315, 90)
            };

            grpRigIdentificacion.Controls.Add(lblRigUsuario);
            grpRigIdentificacion.Controls.Add(txtRigUsuario);
            grpRigIdentificacion.Controls.Add(lblRigCodigo);
            grpRigIdentificacion.Controls.Add(txtRigCodigo);
            grpRigIdentificacion.Controls.Add(btnRigConectar);
            grpRigIdentificacion.Controls.Add(lblRigEstado);

            grpTermOperaciones = new GroupBox
            {
                Text = " [ 02. COMANDOS DE LA ESTACIÓN ] ",
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(415, 92),
                Size = new Size(385, 360)
            };

            btnTermNarrativa = CrearBotonMenu("> BITÁCORA: OPERACIÓN MEMORIA", 42, Color.FromArgb(30, 41, 59), Color.FromArgb(245, 158, 11));
            btnTermNarrativa.Click += (s, e) =>
            {
                using FormNarrativa historia = new FormNarrativa();
                historia.ShowDialog(this);
            };

            btnTermNuevaPartida = CrearBotonMenu("> DESPLEGAR EN CUBIERTA 15x12", 108, Color.FromArgb(37, 99, 235), Color.White);
            btnTermNuevaPartida.Enabled = false;
            btnTermNuevaPartida.Click += BtnTermNuevaPartida_Click;

            btnTermPartidasSQL = CrearBotonMenu("> BÓVEDA DE MEMORIA (PARTIDAS SQL)", 174, Color.FromArgb(30, 41, 59), Color.FromArgb(6, 182, 212));
            btnTermPartidasSQL.Enabled = false;
            btnTermPartidasSQL.Click += BtnTermPartidasSQL_Click;

            btnTermSalir = CrearBotonMenu("> DESCONECTAR TERMINAL", 275, Color.FromArgb(127, 29, 29), Color.White);
            btnTermSalir.Click += (s, e) =>
            {
                DialogResult r = MessageBox.Show("¿Desea cerrar la terminal de DEAD SIGNAL?", "Confirmar salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes) Application.Exit();
            };

            grpTermOperaciones.Controls.Add(btnTermNarrativa);
            grpTermOperaciones.Controls.Add(btnTermNuevaPartida);
            grpTermOperaciones.Controls.Add(btnTermPartidasSQL);
            grpTermOperaciones.Controls.Add(btnTermSalir);

            this.Controls.Add(lblHeader);
            this.Controls.Add(lblSub);
            this.Controls.Add(grpRigIdentificacion);
            this.Controls.Add(grpTermOperaciones);
        }

        private Button CrearBotonMenu(string texto, int posY, Color fondo, Color textoColor)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(22, posY),
                Size = new Size(340, 48),
                FlatStyle = FlatStyle.Flat,
                BackColor = fondo,
                ForeColor = textoColor,
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);
            return btn;
        }

        private void BtnRigConectar_Click(object? sender, EventArgs e)
        {
            string usuario = txtRigUsuario.Text.Trim();
            string codigo = txtRigCodigo.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(codigo))
            {
                MessageBox.Show("Debe ingresar su Alias de Operador y Código RIG para autenticar el enlace.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRigUsuario.Focus();
                return;
            }

            try
            {
                btnRigConectar.Enabled = false;
                btnRigConectar.Text = "CONECTANDO A SQL SERVER...";
                Application.DoEvents();

                var resultado = _gestorBD.ObtenerORegistrarJugador(usuario, codigo);

                idJugadorActivo = resultado.IdJugador;
                cadeteActivo = usuario;
                codigoRigActivo = codigo;
                sesionIniciada = true;

                string tipoRegistro = resultado.EsNuevo ? "NUEVO PERFIL CREADO" : "PERFIL RECUPERADO";
                lblRigEstado.ForeColor = Color.FromArgb(52, 211, 153);
                lblRigEstado.Text = $"ESTADO SQL: CONECTADO [{tipoRegistro}]\nID Operador: #{idJugadorActivo} | {cadeteActivo} [{codigoRigActivo}]\nBóveda DeadSignal_DB operativa.";

                btnTermNuevaPartida.Enabled = true;
                btnTermPartidasSQL.Enabled = true;
                btnRigConectar.Text = $"RIG #{idJugadorActivo} SINCRONIZADO ✓";
                btnRigConectar.Enabled = true;
            }
            catch (Exception ex)
            {
                btnRigConectar.Enabled = true;
                btnRigConectar.Text = "REINTENTAR CONEXIÓN SQL";
                lblRigEstado.ForeColor = Color.FromArgb(248, 113, 113);
                lblRigEstado.Text = "ERROR DE ENLACE SQL SERVER.\nVerifique que DeadSignal_DB esté activa.";

                MessageBox.Show($"No se pudo conectar con SQL Server (DeadSignal_DB):\n\n{ex.Message}",
                    "Fallo de Bóveda de Memoria", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTermNuevaPartida_Click(object? sender, EventArgs e)
        {
            if (!sesionIniciada) return;

            try
            {
                // Crea automáticamente una ranura rápida en SQL Server para este despliegue
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

                string ranuraAuto = $"Despliegue_{DateTime.Now:ddMM_HHmmss}";
                int idPartida = _gestorBD.CrearNuevaPartida(idJugadorActivo, ranuraAuto, estadoInicial, new List<EntidadMapaDTO>());
                PartidaCompletaDTO? partidaNueva = _gestorBD.CargarPartidaCompleta(idPartida);

                AbrirMapaTactico(partidaNueva);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo registrar la nueva partida en SQL Server:\n{ex.Message}",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTermPartidasSQL_Click(object? sender, EventArgs e)
        {
            if (!sesionIniciada) return;

            using FormBovedaMemoria boveda = new FormBovedaMemoria(idJugadorActivo, cadeteActivo, codigoRigActivo);
            if (boveda.ShowDialog(this) == DialogResult.OK && boveda.PartidaSeleccionada != null)
            {
                AbrirMapaTactico(boveda.PartidaSeleccionada);
            }
        }

        private void AbrirMapaTactico(PartidaCompletaDTO? partida)
        {
            FormMapa2D mapaTactico = new FormMapa2D();
            mapaTactico.PartidaActivaSQL = partida;

            string infoRanura = partida != null ? $" | Ranura: {partida.NombreRanura} (#{partida.IdPartida})" : "";
            mapaTactico.Text = $"DEAD SIGNAL — Operador: {cadeteActivo} [{codigoRigActivo}]{infoRanura}";

            this.Hide();
            mapaTactico.FormClosed += (s, args) => this.Show();
            mapaTactico.Show();
        }
    }
}