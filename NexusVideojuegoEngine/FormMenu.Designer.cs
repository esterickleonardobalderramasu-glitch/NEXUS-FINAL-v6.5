namespace NexusVideojuegoEngine
{
    partial class FormMenu
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblBadgeHeader = new System.Windows.Forms.Label();
            this.grpHUD = new System.Windows.Forms.GroupBox();
            this.lblCadeteNombre = new System.Windows.Forms.Label();
            this.lblCadeteRango = new System.Windows.Forms.Label();
            this.lblCadetePosicion = new System.Windows.Forms.Label();
            this.lblCadeteXP = new System.Windows.Forms.Label();
            this.lblSaludEtiqueta = new System.Windows.Forms.Label();
            this.barVida = new System.Windows.Forms.ProgressBar();
            this.lblVidaTexto = new System.Windows.Forms.Label();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnMapa2D = new System.Windows.Forms.Button();
            this.btnRegistro = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
            this.btnBD = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.statusStripBar = new System.Windows.Forms.StatusStrip();
            this.lblRelojStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBDStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrReloj = new System.Windows.Forms.Timer(this.components);

            this.pnlHeader.SuspendLayout();
            this.grpHUD.SuspendLayout();
            this.pnlAcciones.SuspendLayout();
            this.statusStripBar.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(38)))));
            this.pnlHeader.Controls.Add(this.lblBadgeHeader);
            this.pnlHeader.Controls.Add(this.lblSubtitulo);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(804, 75);
            this.pnlHeader.TabIndex = 0;

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.Cyan;
            this.lblTitulo.Location = new System.Drawing.Point(25, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(432, 26);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "NEXUS TACTICAL OPERATING SYSTEM";

            // lblSubtitulo
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.DarkCyan;
            this.lblSubtitulo.Location = new System.Drawing.Point(27, 45);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(392, 14);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "MATRIZ DE OPERACIONES TÁCTICAS Y CONTROL DE PERSISTENCIA";

            // lblBadgeHeader
            this.lblBadgeHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(40)))), ((int)(((byte)(25)))));
            this.lblBadgeHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBadgeHeader.Font = new System.Drawing.Font("Consolas", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBadgeHeader.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblBadgeHeader.Location = new System.Drawing.Point(650, 22);
            this.lblBadgeHeader.Name = "lblBadgeHeader";
            this.lblBadgeHeader.Size = new System.Drawing.Size(120, 28);
            this.lblBadgeHeader.TabIndex = 2;
            this.lblBadgeHeader.Text = "● CORE READY";
            this.lblBadgeHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // grpHUD
            this.grpHUD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(28)))), ((int)(((byte)(44)))));
            this.grpHUD.Controls.Add(this.lblVidaTexto);
            this.grpHUD.Controls.Add(this.barVida);
            this.grpHUD.Controls.Add(this.lblSaludEtiqueta);
            this.grpHUD.Controls.Add(this.lblCadeteXP);
            this.grpHUD.Controls.Add(this.lblCadetePosicion);
            this.grpHUD.Controls.Add(this.lblCadeteRango);
            this.grpHUD.Controls.Add(this.lblCadeteNombre);
            this.grpHUD.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpHUD.ForeColor = System.Drawing.Color.Cyan;
            this.grpHUD.Location = new System.Drawing.Point(25, 90);
            this.grpHUD.Name = "grpHUD";
            this.grpHUD.Size = new System.Drawing.Size(750, 135);
            this.grpHUD.TabIndex = 1;
            this.grpHUD.TabStop = false;
            this.grpHUD.Text = " [ STATUS DEL OPERADOR ACTIVO ] ";

            // lblCadeteNombre
            this.lblCadeteNombre.AutoSize = true;
            this.lblCadeteNombre.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold);
            this.lblCadeteNombre.ForeColor = System.Drawing.Color.White;
            this.lblCadeteNombre.Location = new System.Drawing.Point(20, 30);
            this.lblCadeteNombre.Name = "lblCadeteNombre";
            this.lblCadeteNombre.Size = new System.Drawing.Size(152, 18);
            this.lblCadeteNombre.TabIndex = 0;
            this.lblCadeteNombre.Text = "OPERADOR: Alfa-1";

            // lblCadeteRango
            this.lblCadeteRango.AutoSize = true;
            this.lblCadeteRango.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.lblCadeteRango.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.lblCadeteRango.Location = new System.Drawing.Point(280, 32);
            this.lblCadeteRango.Name = "lblCadeteRango";
            this.lblCadeteRango.Size = new System.Drawing.Size(112, 15);
            this.lblCadeteRango.TabIndex = 1;
            this.lblCadeteRango.Text = "RANGO: Recluta";

            // lblCadetePosicion
            this.lblCadetePosicion.AutoSize = true;
            this.lblCadetePosicion.Font = new System.Drawing.Font("Consolas", 9F);
            this.lblCadetePosicion.ForeColor = System.Drawing.Color.Cyan;
            this.lblCadetePosicion.Location = new System.Drawing.Point(20, 60);
            this.lblCadetePosicion.Name = "lblCadetePosicion";
            this.lblCadetePosicion.Size = new System.Drawing.Size(154, 14);
            this.lblCadetePosicion.TabIndex = 2;
            this.lblCadetePosicion.Text = "MATRIZ (X,Y): (0, 0)";

            // lblCadeteXP
            this.lblCadeteXP.AutoSize = true;
            this.lblCadeteXP.Font = new System.Drawing.Font("Consolas", 9F);
            this.lblCadeteXP.ForeColor = System.Drawing.Color.Yellow;
            this.lblCadeteXP.Location = new System.Drawing.Point(280, 60);
            this.lblCadeteXP.Name = "lblCadeteXP";
            this.lblCadeteXP.Size = new System.Drawing.Size(147, 14);
            this.lblCadeteXP.TabIndex = 3;
            this.lblCadeteXP.Text = "XP ACUMULADA: 0 PTS";

            // lblSaludEtiqueta
            this.lblSaludEtiqueta.AutoSize = true;
            this.lblSaludEtiqueta.Font = new System.Drawing.Font("Consolas", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblSaludEtiqueta.ForeColor = System.Drawing.Color.Gray;
            this.lblSaludEtiqueta.Location = new System.Drawing.Point(20, 92);
            this.lblSaludEtiqueta.Name = "lblSaludEtiqueta";
            this.lblSaludEtiqueta.Size = new System.Drawing.Size(112, 14);
            this.lblSaludEtiqueta.TabIndex = 4;
            this.lblSaludEtiqueta.Text = "VITALIDAD CORE:";

            // barVida
            this.barVida.Location = new System.Drawing.Point(145, 90);
            this.barVida.Name = "barVida";
            this.barVida.Size = new System.Drawing.Size(460, 20);
            this.barVida.TabIndex = 5;
            this.barVida.Value = 100;

            // lblVidaTexto
            this.lblVidaTexto.AutoSize = true;
            this.lblVidaTexto.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.lblVidaTexto.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblVidaTexto.Location = new System.Drawing.Point(615, 92);
            this.lblVidaTexto.Name = "lblVidaTexto";
            this.lblVidaTexto.Size = new System.Drawing.Size(91, 14);
            this.lblVidaTexto.TabIndex = 6;
            this.lblVidaTexto.Text = "100 / 100 HP";

            // pnlAcciones
            this.pnlAcciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(34)))));
            this.pnlAcciones.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAcciones.Controls.Add(this.btnSalir);
            this.pnlAcciones.Controls.Add(this.btnBD);
            this.pnlAcciones.Controls.Add(this.btnReportes);
            this.pnlAcciones.Controls.Add(this.btnRegistro);
            this.pnlAcciones.Controls.Add(this.btnMapa2D);
            this.pnlAcciones.Location = new System.Drawing.Point(25, 240);
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(750, 320);
            this.pnlAcciones.TabIndex = 2;

            // btnMapa2D
            this.btnMapa2D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(50)))));
            this.btnMapa2D.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMapa2D.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
            this.btnMapa2D.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapa2D.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnMapa2D.ForeColor = System.Drawing.Color.Cyan;
            this.btnMapa2D.Location = new System.Drawing.Point(35, 15);
            this.btnMapa2D.Name = "btnMapa2D";
            this.btnMapa2D.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnMapa2D.Size = new System.Drawing.Size(675, 44);
            this.btnMapa2D.TabIndex = 0;
            this.btnMapa2D.Text = "1. DESPLIEGUE A MATRIZ TÁCTICA 2D (15x12)";
            this.btnMapa2D.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMapa2D.UseVisualStyleBackColor = false;
            this.btnMapa2D.Click += new System.EventHandler(this.btnMapa2D_Click);
            this.btnMapa2D.MouseEnter += new System.EventHandler(this.Boton_MouseEnter);
            this.btnMapa2D.MouseLeave += new System.EventHandler(this.Boton_MouseLeave);

            // btnRegistro
            this.btnRegistro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(50)))));
            this.btnRegistro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegistro.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
            this.btnRegistro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistro.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRegistro.ForeColor = System.Drawing.Color.Cyan;
            this.btnRegistro.Location = new System.Drawing.Point(35, 75);
            this.btnRegistro.Name = "btnRegistro";
            this.btnRegistro.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnRegistro.Size = new System.Drawing.Size(675, 44);
            this.btnRegistro.TabIndex = 1;
            this.btnRegistro.Text = "2. REGISTRO Y GESTIÓN DE OPERADORES";
            this.btnRegistro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegistro.UseVisualStyleBackColor = false;
            this.btnRegistro.Click += new System.EventHandler(this.btnRegistro_Click);
            this.btnRegistro.MouseEnter += new System.EventHandler(this.Boton_MouseEnter);
            this.btnRegistro.MouseLeave += new System.EventHandler(this.Boton_MouseLeave);

            // btnReportes
            this.btnReportes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(50)))));
            this.btnReportes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReportes.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
            this.btnReportes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportes.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnReportes.ForeColor = System.Drawing.Color.Cyan;
            this.btnReportes.Location = new System.Drawing.Point(35, 135);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnReportes.Size = new System.Drawing.Size(675, 44);
            this.btnReportes.TabIndex = 2;
            this.btnReportes.Text = "3. MÓDULO DE REPORTES Y DATASETS (U6)";
            this.btnReportes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportes.UseVisualStyleBackColor = false;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            this.btnReportes.MouseEnter += new System.EventHandler(this.Boton_MouseEnter);
            this.btnReportes.MouseLeave += new System.EventHandler(this.Boton_MouseLeave);

            // btnBD
            this.btnBD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(50)))));
            this.btnBD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBD.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
            this.btnBD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBD.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBD.ForeColor = System.Drawing.Color.Cyan;
            this.btnBD.Location = new System.Drawing.Point(35, 195);
            this.btnBD.Name = "btnBD";
            this.btnBD.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnBD.Size = new System.Drawing.Size(675, 44);
            this.btnBD.TabIndex = 3;
            this.btnBD.Text = "4. COMPROBAR CONEXIÓN SQL SERVER EXPRESS";
            this.btnBD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBD.UseVisualStyleBackColor = false;
            this.btnBD.Click += new System.EventHandler(this.btnBD_Click);
            this.btnBD.MouseEnter += new System.EventHandler(this.Boton_MouseEnter);
            this.btnBD.MouseLeave += new System.EventHandler(this.Boton_MouseLeave);

            // btnSalir
            this.btnSalir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(50)))));
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSalir.ForeColor = System.Drawing.Color.Cyan;
            this.btnSalir.Location = new System.Drawing.Point(35, 255);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnSalir.Size = new System.Drawing.Size(675, 44);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "5. DESCONECTAR / SALIR DEL SISTEMA";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            this.btnSalir.MouseEnter += new System.EventHandler(this.Boton_MouseEnter);
            this.btnSalir.MouseLeave += new System.EventHandler(this.Boton_MouseLeave);

            // statusStripBar
            this.statusStripBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(38)))));
            this.statusStripBar.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.statusStripBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblRelojStatus,
            this.lblBDStatus});
            this.statusStripBar.Location = new System.Drawing.Point(0, 580);
            this.statusStripBar.Name = "statusStripBar";
            this.statusStripBar.Size = new System.Drawing.Size(804, 22);
            this.statusStripBar.TabIndex = 3;

            // lblRelojStatus
            this.lblRelojStatus.Font = new System.Drawing.Font("Consolas", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblRelojStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lblRelojStatus.Name = "lblRelojStatus";
            this.lblRelojStatus.Size = new System.Drawing.Size(175, 17);
            this.lblRelojStatus.Text = "SISTEMA: INICIALIZANDO...";

            // lblBDStatus
            this.lblBDStatus.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblBDStatus.Name = "lblBDStatus";
            this.lblBDStatus.Size = new System.Drawing.Size(259, 17);
            this.lblBDStatus.Text = " | BD: SQL SERVER EXPRESS [NexusV65DB]";

            // tmrReloj
            this.tmrReloj.Enabled = true;
            this.tmrReloj.Interval = 1000;
            this.tmrReloj.Tick += new System.EventHandler(this.tmrReloj_Tick);

            // FormMenu
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(14)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(804, 602);
            this.Controls.Add(this.statusStripBar);
            this.Controls.Add(this.pnlAcciones);
            this.Controls.Add(this.grpHUD);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NEXUS COMMAND CENTER v6.6 — Matrix Interface";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpHUD.ResumeLayout(false);
            this.grpHUD.PerformLayout();
            this.pnlAcciones.ResumeLayout(false);
            this.statusStripBar.ResumeLayout(false);
            this.statusStripBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblBadgeHeader;
        private System.Windows.Forms.GroupBox grpHUD;
        private System.Windows.Forms.Label lblCadeteNombre;
        private System.Windows.Forms.Label lblCadeteRango;
        private System.Windows.Forms.Label lblCadetePosicion;
        private System.Windows.Forms.Label lblCadeteXP;
        private System.Windows.Forms.Label lblSaludEtiqueta;
        private System.Windows.Forms.ProgressBar barVida;
        private System.Windows.Forms.Label lblVidaTexto;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnMapa2D;
        private System.Windows.Forms.Button btnRegistro;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Button btnBD;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.StatusStrip statusStripBar;
        private System.Windows.Forms.ToolStripStatusLabel lblRelojStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblBDStatus;
        private System.Windows.Forms.Timer tmrReloj;
    }
}