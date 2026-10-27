using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public class FormMenu : Form
    {
        private Cadete jugadorCadete = new Cadete("NEXUS", 1, 1, 100);
        private int energiaCore = 35;
        private GestorBD gestorBD = new GestorBD();

        public FormMenu()
        {
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "NEXUS OS v6.5 — Menú Principal de Operaciones (GUI)";
            this.Size = new Size(650, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Encabezado
            Label lblTitulo = new Label
            {
                Text = "NEXUS ENGINE v6.5",
                Font = new Font("Consolas", 20, FontStyle.Bold),
                ForeColor = Color.Cyan,
                AutoSize = true,
                Location = new Point(180, 25)
            };
            this.Controls.Add(lblTitulo);

            Label lblSubtitulo = new Label
            {
                Text = "SISTEMA DE CONTROL Y SIMULACIÓN DE MISIONES (2297)",
                Font = new Font("Consolas", 9, FontStyle.Regular),
                ForeColor = Color.DarkCyan,
                AutoSize = true,
                Location = new Point(140, 65)
            };
            this.Controls.Add(lblSubtitulo);

            int btnTop = 110;
            int btnSpacing = 50;

            // Botón 1: Mapa 2D
            Button btnMapa2D = CrearBoton("1. DESPLIEGUE A MATRIZ DE SIMULACIÓN 2D", new Point(120, btnTop));
            btnMapa2D.Click += (s, e) =>
            {
                FormMapa2D mapa = new FormMapa2D();
                mapa.ShowDialog();
            };
            this.Controls.Add(btnMapa2D);

            // Botón 2: Persistencia SQL
            btnTop += btnSpacing;
            Button btnGuardarBD = CrearBoton("2. GUARDAR PARTIDA EN SQL SERVER (T-SQL)", new Point(120, btnTop));
            btnGuardarBD.Click += (s, e) =>
            {
                bool exito = gestorBD.GuardarPartidaTransaccional(jugadorCadete, energiaCore);
                if (exito)
                {
                    MessageBox.Show("¡Estado del Cadete y evento de Bitácora guardados exitosamente mediante Transacción T-SQL!", 
                                    "Persistencia SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se pudo conectar a SQL Server. Asegúrate de tener LocalDB o SQL Express en ejecución.", 
                                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            this.Controls.Add(btnGuardarBD);

            // Botón 3: Perfil
            btnTop += btnSpacing;
            Button btnPerfil = CrearBoton("3. PERFIL DEL CADETE Y ESTADÍSTICAS", new Point(120, btnTop));
            btnPerfil.Click += (s, e) =>
            {
                MessageBox.Show($"Cadete: {jugadorCadete.Nombre}\n" +
                                $"Rango: Nivel {jugadorCadete.Rango}\n" +
                                $"Salud: {jugadorCadete.Vida}/{jugadorCadete.VidaMax} HP\n" +
                                $"Energía Core: {energiaCore}%\n" +
                                $"Experiencia: {jugadorCadete.XP} XP", 
                                "Perfil del Explorador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            this.Controls.Add(btnPerfil);

            // Botón 4: Unidad 5 (Eventos, Validaciones y Archivos)
            btnTop += btnSpacing;
            Button btnUnidad5 = CrearBoton("4. UNIDAD 5: REGISTRO Y ARCHIVOS (.TXT)", new Point(120, btnTop));
            btnUnidad5.Click += (s, e) =>
            {
                FormRegistroCadete formU5 = new FormRegistroCadete();
                formU5.ShowDialog();
            };
            this.Controls.Add(btnUnidad5);

            // Botón 5: Salir
            btnTop += btnSpacing;
            Button btnSalir = CrearBoton("5. SALIR DEL SISTEMA", new Point(120, btnTop));
            btnSalir.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnSalir);
        }

        private Button CrearBoton(string texto, Point ubicacion)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = ubicacion,
                Size = new Size(400, 40),
                BackColor = Color.FromArgb(25, 35, 50),
                ForeColor = Color.Cyan,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Cyan;
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(40, 60, 90);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(25, 35, 50);
            return btn;
        }
    }
}