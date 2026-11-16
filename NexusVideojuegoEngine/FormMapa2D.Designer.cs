namespace NexusVideojuegoEngine
{
    partial class FormMapa2D
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmrSimulacion = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrSimulacion
            // 
            this.tmrSimulacion.Enabled = true;
            this.tmrSimulacion.Interval = 450;
            this.tmrSimulacion.Tick += new System.EventHandler(this.tmrSimulacion_Tick);
            // 
            // FormMapa2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(14)))), ((int)(((byte)(22)))));
            this.ClientSize = new System.Drawing.Size(820, 610);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMapa2D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NEXUS TACTICAL MAP 2D — Matriz de Operaciones (15x12)";
            this.Load += new System.EventHandler(this.FormMapa2D_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMapa2D_Paint);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer tmrSimulacion;
    }
}