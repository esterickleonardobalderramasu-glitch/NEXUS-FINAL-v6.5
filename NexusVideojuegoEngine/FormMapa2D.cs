using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public class FormMapa2D : Form
    {
        private const int TamanoCelda = 40;
        private int playerX = 1;
        private int playerY = 1;

        private readonly int[,] mapaAlfa = new int[8, 16]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 1, 1, 4, 1, 1, 1, 0, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 2, 1, 0, 0, 5, 1, 0, 0, 1 },
            { 1, 2, 1, 1, 1, 0, 1, 0, 1, 0, 3, 0, 1, 0, 0, 1 },
            { 1, 0, 0, 3, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 2, 0, 0, 0, 1, 2, 0, 0, 0, 2, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        public FormMapa2D()
        {
            this.Text = "NEXUS OS v6.5 — Exploración de Sector 2D (GUI)";
            this.ClientSize = new Size(16 * TamanoCelda, 8 * TamanoCelda + 40);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(15, 20, 30);
            this.KeyDown += FormMapa2D_KeyDown;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            int filas = mapaAlfa.GetLength(0);
            int columnas = mapaAlfa.GetLength(1);

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    Rectangle rect = new Rectangle(x * TamanoCelda, y * TamanoCelda, TamanoCelda, TamanoCelda);
                    int celda = mapaAlfa[y, x];

                    if (celda == 1) // Pared / Muro
                    {
                        g.FillRectangle(Brushes.DarkSlateGray, rect);
                        g.DrawRectangle(Pens.Cyan, rect);
                    }
                    else // Caminito / Suelo
                    {
                        g.FillRectangle(Brushes.Black, rect);
                        g.DrawRectangle(Pens.DarkBlue, rect);
                    }
                }
            }

            // Dibujar al Jugador (Cadete) como un avatar gráfico brillante
            Rectangle playerRect = new Rectangle(playerX * TamanoCelda + 5, playerY * TamanoCelda + 5, TamanoCelda - 10, TamanoCelda - 10);
            g.FillEllipse(Brushes.SpringGreen, playerRect);
            g.DrawEllipse(Pens.White, playerRect);

            // Barra inferior de estado
            string info = $" [NEXUS CADETE] Posición: ({playerX}, {playerY}) │ Usa Flechas / WASD para moverte";
            g.DrawString(info, new Font("Consolas", 10, FontStyle.Bold), Brushes.Cyan, 10, 8 * TamanoCelda + 10);
        }

        private void FormMapa2D_KeyDown(object? sender, KeyEventArgs e)
        {
            int nx = playerX;
            int ny = playerY;

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) ny--;
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) ny++;
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) nx--;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) nx++;

            if (nx >= 0 && nx < 16 && ny >= 0 && ny < 8 && mapaAlfa[ny, nx] != 1)
            {
                playerX = nx;
                playerY = ny;
                this.Invalidate(); // Redibujar pantalla
            }
        }
    }
}