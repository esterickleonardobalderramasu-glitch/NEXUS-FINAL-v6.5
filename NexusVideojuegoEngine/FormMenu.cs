using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public partial class FormMenu : Form
    {
        private Cadete cadeteActual;
        private readonly GestorBD gestorBD;

        public FormMenu()
        {
            InitializeComponent();

            gestorBD = new GestorBD();
            cadeteActual = new Cadete("Alfa-1", 0, 0, 100);

            RefrescarHUD();
        }

        private void RefrescarHUD()
        {
            lblCadeteNombre.Text = $"OPERADOR: {cadeteActual.Nombre}";
            lblCadeteRango.Text = $"RANGO: {cadeteActual.Rango}";
            lblCadetePosicion.Text = $"MATRIZ (X,Y): ({cadeteActual.PosX}, {cadeteActual.PosY})";
            lblCadeteXP.Text = $"XP ACUMULADA: {cadeteActual.XP} PTS";

            int vida = Math.Clamp(cadeteActual.Vida, 0, cadeteActual.VidaMax);
            barVida.Value = vida;
            lblVidaTexto.Text = $"{vida} / {cadeteActual.VidaMax} HP";
        }

        private void tmrReloj_Tick(object? sender, EventArgs e)
        {
            lblRelojStatus.Text = $"SISTEMA: {DateTime.Now:HH:mm:ss}  |  STATUS ENGINE: ACTIVE";
        }

        private void btnMapa2D_Click(object? sender, EventArgs e)
        {
            FormMapa2D mapa = new FormMapa2D(cadeteActual);
            mapa.ShowDialog();
            RefrescarHUD();
        }

        private void btnRegistro_Click(object? sender, EventArgs e)
        {
            FormRegistroCadete registro = new FormRegistroCadete();
            registro.ShowDialog();
            RefrescarHUD();
        }

        private void btnReportes_Click(object? sender, EventArgs e)
        {
            FormReportesYDataSets reportes = new FormReportesYDataSets();
            reportes.ShowDialog();
            RefrescarHUD();
        }

        private void btnBD_Click(object? sender, EventArgs e)
        {
            bool ok = gestorBD.ProbarConexion();
            if (ok)
                MessageBox.Show("Conexión activa con la base de datos NexusV65DB.", "Persistencia SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo conectar a la base de datos.", "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            RefrescarHUD();
        }

        private void btnSalir_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Boton_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb(32, 54, 82);
                btn.ForeColor = Color.SpringGreen;
                btn.FlatAppearance.BorderColor = Color.SpringGreen;
            }
        }

        private void Boton_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb(22, 33, 50);
                btn.ForeColor = Color.Cyan;
                btn.FlatAppearance.BorderColor = Color.FromArgb(0, 180, 216);
            }
        }
    }
}