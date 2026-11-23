using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public partial class FormMenu : Form
    {
        // Controles de Login / Perfil de Cadete
        private GroupBox grpRigIdentificacion = null!;
        private Label lblRigUsuario = null!;
        private TextBox txtRigUsuario = null!;
        private Label lblRigCodigo = null!;
        private TextBox txtRigCodigo = null!;
        private Button btnRigConectar = null!;
        private Label lblRigEstado = null!;

        // Controles del Menú Principal
        private GroupBox grpTermOperaciones = null!;
        private Button btnTermNarrativa = null!;
        private Button btnTermNuevaPartida = null!;
        private Button btnTermPartidasSQL = null!;
        private Button btnTermSalir = null!;

        // Datos del operador autenticado
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

            this.Text = "NEXUS-09 // TERMINAL DE ACCESO Y MANDO (v6.7)";
            this.Size = new Size(840, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(12, 18, 32);

            Label lblHeader = new Label
            {
                Text = "ESTACIÓN ORBITAL NEXUS-09  |  SISTEMA DE CONTENCIÓN",
                Font = new Font("Consolas", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(28, 22),
                AutoSize = true
            };

            Label lblSub = new Label
            {
                Text = "PROTOCOLO ACTIVO: OPERACIÓN MEMORIA — IDENTIFIQUE SU TRAJE RIG PARA CONTINUAR",
                Font = new Font("Consolas", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(30, 52),
                AutoSize = true
            };

            // =================================================================
            // PANEL IZQUIERDO: LOGIN / REGISTRO DE PERFIL DE CADETE
            // =================================================================
            grpRigIdentificacion = new GroupBox
            {
                Text = " [ 01. ACCESO DE OPERADOR (RIG) ] ",
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 182, 212),
                Location = new Point(28, 92),
                Size = new Size(365, 360)
            };

            lblRigUsuario = new Label
            {
                Text = "ALIAS DEL CADETE:",
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
                Text = "CÓDIGO DE TRAJE RIG (Ej. EXO-09):",
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
                Text = "EXO-09"
            };

            btnRigConectar = new Button
            {
                Text = "SINCRONIZAR TARJETA RIG",
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
                Text = "ESTADO: ENLACE BLOQUEADO\nIngrese su alias para habilitar los sistemas.",
                Font = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(248, 113, 113),
                Location = new Point(22, 255),
                Size = new Size(315, 75)
            };

            grpRigIdentificacion.Controls.Add(lblRigUsuario);
            grpRigIdentificacion.Controls.Add(txtRigUsuario);
            grpRigIdentificacion.Controls.Add(lblRigCodigo);
            grpRigIdentificacion.Controls.Add(txtRigCodigo);
            grpRigIdentificacion.Controls.Add(btnRigConectar);
            grpRigIdentificacion.Controls.Add(lblRigEstado);

            // =================================================================
            // PANEL DERECHO: MENÚ PRINCIPAL DE OPERACIONES
            // =================================================================
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

            btnTermNuevaPartida = CrearBotonMenu("> DESPLEGAR EN SECTOR 15x12", 108, Color.FromArgb(37, 99, 235), Color.White);
            btnTermNuevaPartida.Enabled = false;
            btnTermNuevaPartida.Click += BtnTermNuevaPartida_Click;

            btnTermPartidasSQL = CrearBotonMenu("> BÓVEDA DE MEMORIA (SQL / DATOS)", 174, Color.FromArgb(30, 41, 59), Color.FromArgb(6, 182, 212));
            btnTermPartidasSQL.Enabled = false;
            btnTermPartidasSQL.Click += BtnTermPartidasSQL_Click;

            btnTermSalir = CrearBotonMenu("> DESCONECTAR TERMINAL", 275, Color.FromArgb(127, 29, 29), Color.White);
            btnTermSalir.Click += (s, e) =>
            {
                DialogResult r = MessageBox.Show("¿Desea cerrar la terminal de NEXUS-09?", "Confirmar salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                MessageBox.Show("Debe ingresar su Alias de Cadete y Código RIG para autenticar el enlace.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRigUsuario.Focus();
                return;
            }

            cadeteActivo = usuario;
            codigoRigActivo = codigo;
            sesionIniciada = true;

            lblRigEstado.ForeColor = Color.FromArgb(52, 211, 153);
            lblRigEstado.Text = $"ESTADO: ENLACE ACTIVO\nOperador: {cadeteActivo} [{codigoRigActivo}]\nSistemas tácticos desbloqueados.";

            btnTermNuevaPartida.Enabled = true;
            btnTermPartidasSQL.Enabled = true;
            btnRigConectar.Text = "PERFIL SINCRONIZADO ✓";
        }

        private void BtnTermNuevaPartida_Click(object? sender, EventArgs e)
        {
            if (!sesionIniciada) return;

            FormMapa2D mapaTactico = new FormMapa2D();
            mapaTactico.Text = $"NEXUS-09 — Operador: {cadeteActivo} [{codigoRigActivo}]";
            this.Hide();
            mapaTactico.FormClosed += (s, args) => this.Show();
            mapaTactico.Show();
        }

        private void BtnTermPartidasSQL_Click(object? sender, EventArgs e)
        {
            FormReportesYDataSets ventanaDatos = new FormReportesYDataSets();
            ventanaDatos.ShowDialog(this);
        }
    }
}