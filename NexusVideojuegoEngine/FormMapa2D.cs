using System;
using System.Drawing;
using System.Windows.Forms;

namespace NexusVideojuegoEngine
{
    public partial class FormMapa2D : Form
    {
        private Cadete cadeteActual;

        // Constructor 1: Requerido por el Diseñador Visual de Visual Studio
        public FormMapa2D() : this(new Cadete("Alfa-1", 0, 0, 100))
        {
        }

        // Constructor 2: Recibe la instancia activa del Cadete enviada desde FormMenu
        public FormMapa2D(Cadete cadete)
        {
            InitializeComponent();
            this.cadeteActual = cadete ?? new Cadete("Alfa-1", 0, 0, 100);
        }

        private void FormMapa2D_Load(object? sender, EventArgs e)
        {
            // Inicialización de la pantalla de mapa 2D
        }
    }
}