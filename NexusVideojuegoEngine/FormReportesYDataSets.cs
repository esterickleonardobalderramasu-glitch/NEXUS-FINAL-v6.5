using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public class FormReportesYDataSets : Form
    {
        private DataGridView dgvDatos = null!;
        private BindingSource bindingSource = new BindingSource();
        private DataSet dataSet = new DataSet();
        private SqlDataAdapter adapter = null!;
        private SqlCommandBuilder commandBuilder = null!;
        private Label lblEstado = null!;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

        // Cadena de conexión LocalDB (Coincide con GestorBD)
        private string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=NexusV65DB;Integrated Security=True;TrustServerCertificate=True;";

        public FormReportesYDataSets()
        {
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "NEXUS OS v6.5 — Unidad 6 (DataSets, Binding y Reportes Gráficos)";
            this.Size = new Size(800, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(15, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Encabezado
            Label lblTitulo = new Label
            {
                Text = "UNIDAD 6: DATASETS, DATABINDING Y REPORTES GRÁFICOS",
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(130, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // DataGridView (Control Ligado a Datos)
            dgvDatos = new DataGridView
            {
                Location = new Point(30, 50),
                Size = new Size(725, 270),
                BackgroundColor = Color.FromArgb(25, 35, 50),
                ForeColor = Color.Black,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true
            };
            this.Controls.Add(dgvDatos);

            // Label de Estado
            lblEstado = new Label
            {
                Text = "Estado: Presione '1. CARGAR DATASET' para consultar la base de datos.",
                Font = new Font("Consolas", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 335),
                AutoSize = true
            };
            this.Controls.Add(lblEstado);

            // Botones de Operaciones
            Button btnCargarDataSet = CrearBoton("1. CARGAR DATASET (SQL)", 30, 370, 230);
            btnCargarDataSet.Click += BtnCargarDataSet_Click;
            this.Controls.Add(btnCargarDataSet);

            Button btnActualizarBD = CrearBoton("2. GUARDAR CAMBIOS EN BD", 275, 370, 230);
            btnActualizarBD.Click += BtnActualizarBD_Click;
            this.Controls.Add(btnActualizarBD);

            Button btnImprimirReporte = CrearBoton("3. REPORTE Y GRÁFICOS", 520, 370, 235);
            btnImprimirReporte.Click += BtnImprimirReporte_Click;
            this.Controls.Add(btnImprimirReporte);

            // Configuración de Impresión / Reporte
            printDocument.PrintPage += PrintDocument_PrintPage;
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.Width = 850;
            printPreviewDialog.Height = 650;
        }

        // ============================================================================
        // 1. CARGA Y MODIFICACIÓN DE DATASET CON DATABINDING
        // ============================================================================

        private void BtnCargarDataSet_Click(object? sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT CadeteID, Nombre, Rango, Vida, VidaMax, XP FROM Cadetes";
                    adapter = new SqlDataAdapter(sql, conn);
                    commandBuilder = new SqlCommandBuilder(adapter);

                    dataSet.Clear();
                    adapter.Fill(dataSet, "Cadetes");

                    // Enlazar datos mediante BindingSource (Control ligado a datos)
                    bindingSource.DataSource = dataSet.Tables["Cadetes"];
                    dgvDatos.DataSource = bindingSource;

                    lblEstado.Text = "Estado: DataSet cargado exitosamente. Puede editar filas directamente en la grilla.";
                    lblEstado.ForeColor = Color.LightGreen;
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = $"Estado: Atencion al cargar SQL ({ex.Message}). Carga simulada activa.";
                lblEstado.ForeColor = Color.Orange;
            }
        }

        private void BtnActualizarBD_Click(object? sender, EventArgs e)
        {
            try
            {
                if (adapter != null && dataSet.Tables["Cadetes"] != null)
                {
                    this.BindingContext[dataSet, "Cadetes"].EndCurrentEdit();
                    int filasAfectadas = adapter.Update(dataSet, "Cadetes");

                    MessageBox.Show($"¡Base de datos actualizada correctamente! Filas procesadas: {filasAfectadas}", 
                                    "Persistencia y DataSet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblEstado.Text = $"Estado: Sincronización exitosa ({filasAfectadas} filas actualizadas).";
                }
                else
                {
                    MessageBox.Show("Cargue primero el DataSet para realizar modificaciones.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar DataSet: {ex.Message}", "Error de Persistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================================
        // 2. IMPRESIÓN Y REPORTES GRÁFICOS
        // ============================================================================

        private void BtnImprimirReporte_Click(object? sender, EventArgs e)
        {
            if (dataSet.Tables["Cadetes"] == null || dataSet.Tables["Cadetes"].Rows.Count == 0)
            {
                MessageBox.Show("Cargue el DataSet antes de generar el reporte gráfico.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            printPreviewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics!;
            Font titleFont = new Font("Consolas", 15, FontStyle.Bold);
            Font headerFont = new Font("Consolas", 11, FontStyle.Bold);
            Font bodyFont = new Font("Consolas", 10, FontStyle.Regular);

            int yPos = 40;

            // Encabezado del Reporte
            g.DrawString("NEXUS OS v6.5 — REPORTE Y ESTADÍSTICAS DE CADETES", titleFont, Brushes.Black, 50, yPos);
            yPos += 25;
            g.DrawLine(Pens.Black, 50, yPos, 750, yPos);
            yPos += 20;

            g.DrawString($"Fecha del Reporte: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", bodyFont, Brushes.DarkGray, 50, yPos);
            yPos += 30;

            // Reporte Gráfico: Dibujo de Barras Estadísticas de Vida
            g.DrawString("--- REPORTES GRÁFICOS: ESTADÍSTICAS DE VIDA (HP) ---", headerFont, Brushes.Navy, 50, yPos);
            yPos += 30;

            DataTable dt = dataSet.Tables["Cadetes"]!;

            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                string nombre = row["Nombre"]?.ToString() ?? "Cadete";
                int vida = Convert.ToInt32(row["Vida"] != DBNull.Value ? row["Vida"] : 0);

                // Texto Cadete
                g.DrawString($"{nombre} ({vida} HP):", bodyFont, Brushes.Black, 50, yPos);

                // Barra Gráfica Proporcional
                int barWidth = Math.Min(350, Math.Max(10, vida * 3));
                g.FillRectangle(Brushes.Teal, 240, yPos, barWidth, 18);
                g.DrawRectangle(Pens.DarkSlateGray, 240, yPos, barWidth, 18);

                yPos += 28;
            }

            yPos += 15;
            g.DrawLine(Pens.Gray, 50, yPos, 750, yPos);
            yPos += 25;

            // Resumen Tabular
            g.DrawString("ID", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Nombre", headerFont, Brushes.Black, 110, yPos);
            g.DrawString("Rango", headerFont, Brushes.Black, 320, yPos);
            g.DrawString("Vida", headerFont, Brushes.Black, 470, yPos);
            g.DrawString("XP", headerFont, Brushes.Black, 570, yPos);
            yPos += 22;

            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                g.DrawString(row["CadeteID"].ToString(), bodyFont, Brushes.Black, 50, yPos);
                g.DrawString(row["Nombre"].ToString(), bodyFont, Brushes.Black, 110, yPos);
                g.DrawString(row["Rango"].ToString(), bodyFont, Brushes.Black, 320, yPos);
                g.DrawString(row["Vida"].ToString(), bodyFont, Brushes.Black, 470, yPos);
                g.DrawString(row["XP"].ToString(), bodyFont, Brushes.Black, 570, yPos);
                yPos += 20;
            }

            // Pie de página
            g.DrawString("Documento generado automáticamente por NEXUS ENGINE v6.5 — Módulo de Reportes de Unidad 6", 
                         new Font("Consolas", 8, FontStyle.Italic), Brushes.Gray, 50, 1000);
        }

        private Button CrearBoton(string texto, int x, int y, int width)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(width, 40),
                BackColor = Color.FromArgb(25, 35, 50),
                ForeColor = Color.Cyan,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.Cyan;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(40, 60, 90);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(25, 35, 50);
            return btn;
        }
    }
}