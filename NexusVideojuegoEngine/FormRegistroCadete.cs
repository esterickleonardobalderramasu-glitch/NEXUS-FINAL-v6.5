using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public class FormRegistroCadete : Form
    {
        private TextBox txtNombre = null!;
        private TextBox txtVidaInicial = null!;
        private ComboBox cbRango = null!;
        private Label lblEstado = null!;
        private ErrorProvider errorProvider = null!;

        public FormRegistroCadete()
        {
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "NEXUS OS v6.5 — Unidad 5 (Eventos, Validaciones y Archivos)";
            this.Size = new Size(580, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

            // Encabezado
            Label lblTitulo = new Label
            {
                Text = "REGISTRO Y EXPORTACIÓN DE CADETES",
                Font = new Font("Consolas", 13, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(90, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // 1. Campo Nombre (Con evento TextChanged)
            Label lblNombre = CrearLabel("Nombre Cadete:", 30, 75);
            txtNombre = CrearTextBox(180, 72);
            txtNombre.TextChanged += TxtNombre_TextChanged; // Evento
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);

            // 2. Campo Vida (Con evento KeyPress para números)
            Label lblVida = CrearLabel("Vida Inicial (1-100):", 30, 125);
            txtVidaInicial = CrearTextBox(180, 122);
            txtVidaInicial.KeyPress += TxtVidaInicial_KeyPress; // Evento
            this.Controls.Add(lblVida);
            this.Controls.Add(txtVidaInicial);

            // 3. Selección de Rango
            Label lblRango = CrearLabel("Rango Asignado:", 30, 175);
            cbRango = new ComboBox
            {
                Location = new Point(180, 172),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(25, 35, 50),
                ForeColor = Color.Cyan,
                Font = new Font("Consolas", 10)
            };
            cbRango.Items.AddRange(new object[] { "Recluta (Nivel 1)", "Oficial (Nivel 2)", "Comandante (Nivel 3)" });
            cbRango.SelectedIndex = 0;
            this.Controls.Add(lblRango);
            this.Controls.Add(cbRango);

            // 4. Feedback de estado
            lblEstado = new Label
            {
                Text = "Estado: Ingrese los datos del cadete...",
                Font = new Font("Consolas", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 220),
                AutoSize = true
            };
            this.Controls.Add(lblEstado);

            // Botones de Archivos (Exportar/Importar)
            Button btnGuardar = CrearBoton("1. EXPORTAR ARCHIVO (.TXT)", 30, 260);
            btnGuardar.Click += BtnGuardarArchivo_Click;
            this.Controls.Add(btnGuardar);

            Button btnCargar = CrearBoton("2. IMPORTAR ARCHIVO (.TXT)", 280, 260);
            btnCargar.Click += BtnCargarArchivo_Click;
            this.Controls.Add(btnCargar);

            // Evento de cierre de ventana
            this.FormClosing += FormRegistroCadete_FormClosing;
        }

        // ============================================================================
        // MANEJADORES DE EVENTOS Y VALIDACIÓN DE ENTRADA
        // ============================================================================

        private void TxtNombre_TextChanged(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "El nombre es obligatorio.");
                lblEstado.Text = "Estado: ⚠️ Ingrese un nombre válido.";
                lblEstado.ForeColor = Color.Orange;
            }
            else
            {
                errorProvider.SetError(txtNombre, "");
                lblEstado.Text = "Estado: ✔️ Nombre correcto.";
                lblEstado.ForeColor = Color.LightGreen;
            }
        }

        private void TxtVidaInicial_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Bloquea letras o caracteres especiales
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool ValidarEntradas()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el cadete.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtVidaInicial.Text, out int vida) || vida < 1 || vida > 100)
            {
                MessageBox.Show("La vida debe ser un número entero entre 1 y 100.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ============================================================================
        // MANEJO DE ARCHIVOS (System.IO)
        // ============================================================================

        private void BtnGuardarArchivo_Click(object? sender, EventArgs e)
        {
            if (!ValidarEntradas()) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivo de texto (*.txt)|*.txt";
                sfd.FileName = $"Cadete_{txtNombre.Text.Trim()}.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string contenido = $"=== REGISTRO CADETE NEXUS OS ===\n" +
                                       $"Nombre: {txtNombre.Text.Trim()}\n" +
                                       $"Vida: {txtVidaInicial.Text.Trim()}\n" +
                                       $"Rango: {cbRango.SelectedItem}\n" +
                                       $"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    File.WriteAllText(sfd.FileName, contenido);
                    MessageBox.Show("¡Archivo guardado exitosamente!", "Manejo de Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblEstado.Text = $"Estado: Guardado en {Path.GetFileName(sfd.FileName)}";
                }
            }
        }

        private void BtnCargarArchivo_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivo de texto (*.txt)|*.txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] lineas = File.ReadAllLines(ofd.FileName);
                    foreach (string linea in lineas)
                    {
                        if (linea.StartsWith("Nombre:")) txtNombre.Text = linea.Replace("Nombre:", "").Trim();
                        if (linea.StartsWith("Vida:")) txtVidaInicial.Text = linea.Replace("Vida:", "").Trim();
                    }
                    MessageBox.Show("¡Datos del archivo cargados!", "Manejo de Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblEstado.Text = $"Estado: Cargado desde {Path.GetFileName(ofd.FileName)}";
                }
            }
        }

        private void FormRegistroCadete_FormClosing(object? sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Deseas regresar al Menú Principal?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        // Auxiliares de Interfaz
        private Label CrearLabel(string texto, int x, int y)
        {
            return new Label { Text = texto, Font = new Font("Consolas", 10, FontStyle.Bold), ForeColor = Color.Cyan, Location = new Point(x, y), AutoSize = true };
        }

        private TextBox CrearTextBox(int x, int y)
        {
            return new TextBox { Location = new Point(x, y), Width = 200, BackColor = Color.FromArgb(25, 35, 50), ForeColor = Color.Cyan, Font = new Font("Consolas", 10), BorderStyle = BorderStyle.FixedSingle };
        }

        private Button CrearBoton(string texto, int x, int y)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(230, 40),
                BackColor = Color.FromArgb(25, 35, 50),
                ForeColor = Color.Cyan,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Cyan;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(40, 60, 90);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(25, 35, 50);
            return btn;
        }
    }
}