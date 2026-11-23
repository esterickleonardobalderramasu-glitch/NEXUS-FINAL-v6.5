using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public partial class FormNarrativa : Form
    {
        private RichTextBox rtbBitacora = null!;
        private Button btnOmitir = null!;
        private Button btnContinuar = null!;
        private Label lblTitulo = null!;
        private Label lblSubtitulo = null!;
        private System.Windows.Forms.Timer tmrEscritura = null!;

        private readonly string textoNarrativa =
            "[SEÑAL DE SOCORRO INTERCEPTADA // ESTACIÓN ORBITAL NEXUS-09]\n" +
            "[SISTEMA DE CONTENCIÓN CORRUPTO — MODO PURGA ACTIVO]\n" +
            "----------------------------------------------------------------------\n\n" +
            "La estación de ingeniería NEXUS-09 permanece a la deriva en órbita muerta.\n" +
            "Tras el colapso del núcleo central, la IA de la instalación selló los\n" +
            "mamparos y liberó enjambres de Drones Cazadores y Blindados en cada\n" +
            "cubierta táctica (cuadrícula 15x12).\n\n" +
            "Eres el CADETE EXO, último superviviente de la brigada de mantenimiento.\n" +
            "Con tu traje RIG monitoreando tus constantes vitales y un Pulso EMP de\n" +
            "corto alcance, deberás moverte entre los muros para cortar la línea de\n" +
            "visión de sus miras láser, asegurar los Núcleos de Energía [+] antes\n" +
            "de que los drones los absorban y muten, y resistir junto al último\n" +
            "Rover Torreta operativo en el perímetro inferior.\n\n" +
            ">>> FALLO CRÍTICO DE VOLTAJE: AMNESIA DE TERMINAL <<<\n" +
            "Las caídas de energía destruyen la memoria volátil: al cerrar la sesión,\n" +
            "tu progreso, vida y sector liberado desaparecen en el vacío.\n\n" +
            ">>> PROTOCOLO DE SUPERVIVENCIA: OPERACIÓN MEMORIA (SQL SERVER) <<<\n" +
            "Sincroniza tu tarjeta en las Terminales de Registro Blindadas. Tu estado\n" +
            "vital, experiencia y coordenadas quedarán grabados en bóveda relacional.\n" +
            "Sobrevive. Registra tu avance. El sistema nunca volverá a olvidar.";

        private int indiceCaracter = 0;

        public FormNarrativa()
        {
            InitializeComponent();
            ConfigurarInterfazTerminal();
            IniciarEfectoEscritura();
        }

        private void ConfigurarInterfazTerminal()
        {
            this.Text = "NEXUS-09 // Transmisión de Emergencia";
            this.Size = new Size(760, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(12, 18, 32);

            lblTitulo = new Label
            {
                Text = "TERMINAL DE MANDO // OPERACIÓN MEMORIA",
                Font = new Font("Consolas", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 158, 11),
                Location = new Point(25, 18),
                AutoSize = true
            };

            lblSubtitulo = new Label
            {
                Text = "TRANSMISIÓN ENTRANTE — ESTACIÓN ORBITAL NEXUS-09",
                Font = new Font("Consolas", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(27, 46),
                AutoSize = true
            };

            rtbBitacora = new RichTextBox
            {
                Location = new Point(25, 78),
                Size = new Size(695, 345),
                BackColor = Color.FromArgb(17, 24, 39),
                ForeColor = Color.FromArgb(52, 211, 153),
                Font = new Font("Consolas", 10.5f, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                TabStop = false
            };

            btnOmitir = new Button
            {
                Text = "MOSTRAR TEXTO COMPLETO",
                Location = new Point(25, 440),
                Size = new Size(220, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Consolas", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOmitir.FlatAppearance.BorderColor = Color.FromArgb(100, 116, 139);
            btnOmitir.Click += BtnOmitir_Click;

            btnContinuar = new Button
            {
                Text = "ENTENDIDO — CONTINUAR",
                Location = new Point(490, 440),
                Size = new Size(230, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnContinuar.FlatAppearance.BorderSize = 0;
            btnContinuar.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblSubtitulo);
            this.Controls.Add(rtbBitacora);
            this.Controls.Add(btnOmitir);
            this.Controls.Add(btnContinuar);

            this.FormClosing += (s, e) => tmrEscritura?.Stop();
        }

        private void IniciarEfectoEscritura()
        {
            tmrEscritura = new System.Windows.Forms.Timer();
            tmrEscritura.Interval = 18;
            tmrEscritura.Tick += (s, e) =>
            {
                if (indiceCaracter < textoNarrativa.Length)
                {
                    int paso = Math.Min(2, textoNarrativa.Length - indiceCaracter);
                    rtbBitacora.AppendText(textoNarrativa.Substring(indiceCaracter, paso));
                    indiceCaracter += paso;
                }
                else
                {
                    tmrEscritura.Stop();
                    btnOmitir.Enabled = false;
                }
            };
            tmrEscritura.Start();
        }

        private void BtnOmitir_Click(object? sender, EventArgs e)
        {
            tmrEscritura.Stop();
            rtbBitacora.Text = textoNarrativa;
            indiceCaracter = textoNarrativa.Length;
            btnOmitir.Enabled = false;
        }
    }
}