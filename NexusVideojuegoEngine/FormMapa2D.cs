#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexusVideojuegoEngine.Data;
using NexusVideojuegoEngine.Models;

namespace NexusVideojuegoEngine
{
    public partial class FormMapa2D : Form
    {
        public PartidaCompletaDTO? PartidaActivaSQL { get; set; }
        private readonly GestorBD _gestorBD = new GestorBD();

        private const int COLS = 15;
        private const int FILAS = 12;
        private const int TAM_CELDA = 40;
        private const int OFFSET_X = 25;
        private const int OFFSET_Y = 70;

        private class DronEnemigo
        {
            public Point Posicion { get; set; }
            public int Vida { get; set; }
            public int VidaMax { get; set; }
            public bool EnAlerta { get; set; }
            public bool ApuntandoLaser { get; set; }
            public Point ObjetivoLaser { get; set; }
            public int TurnosAturdido { get; set; }
            public bool Sobrecargado { get; set; }

            public DronEnemigo(int x, int y, bool blindado = false)
            {
                Posicion = new Point(x, y);
                VidaMax = blindado ? 2 : 1;
                Vida = VidaMax;
            }
        }

        private class RayoVisual
        {
            public Point Inicio { get; set; }
            public Point Fin { get; set; }
            public Color ColorRayo { get; set; }
            public int FramesRestantes { get; set; }
        }

        private readonly Cadete cadeteActual;
        private int vidaActual;
        private int vidaMaxActual;
        private int xpActual;
        private int turnosJugados = 0;
        private int cargasEMP = 2;
        private const int MAX_CARGAS_EMP = 3;
        private readonly Random rng = new Random();

        private readonly List<Point> paredes = new List<Point>();
        private readonly List<DronEnemigo> enemigos = new List<DronEnemigo>();
        private readonly List<Point> nucleosEnergia = new List<Point>();
        private readonly List<RayoVisual> rayosActivos = new List<RayoVisual>();

        private Point robotAliado = new Point(2, 10);
        private int dirRobot = 1;
        private readonly Point naveExtraccion = new Point(14, 11);

        private int dirX = 1;
        private int dirY = 0;
        private int animFrame = 0;
        private int ticksSimulacion = 0;
        private int pulsoVisualFrames = 0;
        private int danoFlashFrames = 0;

        private string ultimoEvento = "SISTEMA TÁCTICO ACTIVO — CUBRETE TRAS MUROS PARA EVITAR LÁSERES HOSTILES.";
        private string estadoSyncSQL = "SQL: SIN RANURA";

        private Button btnGuardarSQL = null!;

        public FormMapa2D() : this(new Cadete("Alfa-1", 0, 0, 100))
        {
        }

        public FormMapa2D(Cadete cadete)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.cadeteActual = cadete ?? new Cadete("Alfa-1", 0, 0, 100);
            this.vidaActual = this.cadeteActual.Vida;
            this.vidaMaxActual = this.cadeteActual.VidaMax;
            this.xpActual = this.cadeteActual.XP;

            this.tmrSimulacion.Interval = 110;
            InicializarEntidades();
            ConfigurarBotonGuardadoSQL();
        }

        private void ConfigurarBotonGuardadoSQL()
        {
            int panelX = OFFSET_X + (COLS * TAM_CELDA) + 18;
            btnGuardarSQL = new Button
            {
                Text = "[G] GUARDAR SQL",
                Location = new Point(panelX + 10, OFFSET_Y + 372),
                Size = new Size(142, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnGuardarSQL.FlatAppearance.BorderSize = 0;
            btnGuardarSQL.Click += (s, e) =>
            {
                SincronizarEstadoConSQL(silencioso: false);
                this.Focus();
            };

            this.Controls.Add(btnGuardarSQL);
        }

        private void FormMapa2D_Load(object? sender, EventArgs e)
        {
            RestaurarPartidaDesdeSQL();
            this.Focus();
        }

        // =========================================================================
        // PERSISTENCIA RELACIONAL SQL SERVER (v7.1)
        // =========================================================================

        private void RestaurarPartidaDesdeSQL()
        {
            if (PartidaActivaSQL == null)
            {
                estadoSyncSQL = "MODO LOCAL (SIN SQL)";
                btnGuardarSQL.Enabled = false;
                return;
            }

            btnGuardarSQL.Enabled = true;
            turnosJugados = PartidaActivaSQL.TurnosJugados;
            vidaMaxActual = Math.Max(1, PartidaActivaSQL.Cadete.VidaMax);
            vidaActual = Math.Clamp(PartidaActivaSQL.Cadete.VidaActual, 1, vidaMaxActual);
            xpActual = PartidaActivaSQL.Cadete.ExperienciaXP;
            cargasEMP = Math.Clamp(PartidaActivaSQL.Cadete.CargasEMP, 0, MAX_CARGAS_EMP);

            int posX = Math.Clamp(PartidaActivaSQL.Cadete.PosicionX, 0, COLS - 1);
            int posY = Math.Clamp(PartidaActivaSQL.Cadete.PosicionY, 0, FILAS - 1);
            if (!paredes.Contains(new Point(posX, posY)))
            {
                cadeteActual.PosX = posX;
                cadeteActual.PosY = posY;
            }

            if (PartidaActivaSQL.Entidades != null && PartidaActivaSQL.Entidades.Count > 0)
            {
                enemigos.Clear();
                nucleosEnergia.Clear();

                foreach (EntidadMapaDTO ent in PartidaActivaSQL.Entidades)
                {
                    if (ent.TipoEntidad.StartsWith("Dron"))
                    {
                        bool esBlindado = ent.TipoEntidad == "DronBlindado" || ent.TipoEntidad == "DronSobrecargado";
                        DronEnemigo dron = new DronEnemigo(ent.PosX, ent.PosY, esBlindado)
                        {
                            Vida = Math.Max(1, ent.Vida),
                            Sobrecargado = ent.TipoEntidad == "DronSobrecargado"
                        };
                        if (dron.Sobrecargado) dron.VidaMax = Math.Max(dron.VidaMax, dron.Vida);

                        if (ent.Estado.StartsWith("Aturdido:") && int.TryParse(ent.Estado.Split(':')[1], out int turnosAt))
                        {
                            dron.TurnosAturdido = turnosAt;
                        }

                        enemigos.Add(dron);
                    }
                    else if (ent.TipoEntidad == "NucleoEnergia")
                    {
                        nucleosEnergia.Add(new Point(ent.PosX, ent.PosY));
                    }
                    else if (ent.TipoEntidad == "RoverAliado")
                    {
                        robotAliado = new Point(ent.PosX, ent.PosY);
                        dirRobot = ent.Estado == "Izquierda" ? -1 : 1;
                    }
                }

                estadoSyncSQL = $"SQL: RANURA #{PartidaActivaSQL.IdPartida}";
                ultimoEvento = $"[BÓVEDA SQL] PARTIDA '{PartidaActivaSQL.NombreRanura}' RESTAURADA (TURNO {turnosJugados}).";
            }
            else
            {
                SincronizarEstadoConSQL(silencioso: true);
                ultimoEvento = $"[BÓVEDA SQL] ENLACE ACTIVO — RANURA '{PartidaActivaSQL.NombreRanura}' INICIALIZADA.";
            }

            this.Invalidate();
        }

        private void SincronizarEstadoConSQL(bool silencioso)
        {
            if (PartidaActivaSQL == null) return;

            try
            {
                PartidaActivaSQL.TurnosJugados = turnosJugados;
                PartidaActivaSQL.Cadete.VidaActual = vidaActual;
                PartidaActivaSQL.Cadete.VidaMax = vidaMaxActual;
                PartidaActivaSQL.Cadete.ExperienciaXP = xpActual;
                PartidaActivaSQL.Cadete.Nivel = 1 + (xpActual / 100);
                PartidaActivaSQL.Cadete.PosicionX = cadeteActual.PosX;
                PartidaActivaSQL.Cadete.PosicionY = cadeteActual.PosY;
                PartidaActivaSQL.Cadete.CargasEMP = cargasEMP;

                List<EntidadMapaDTO> entidadesSnapshot = new List<EntidadMapaDTO>();

                foreach (DronEnemigo d in enemigos)
                {
                    string tipo = d.Sobrecargado ? "DronSobrecargado" : (d.VidaMax > 1 ? "DronBlindado" : "DronExplorador");
                    entidadesSnapshot.Add(new EntidadMapaDTO
                    {
                        TipoEntidad = tipo,
                        PosX = d.Posicion.X,
                        PosY = d.Posicion.Y,
                        Vida = d.Vida,
                        Estado = d.TurnosAturdido > 0 ? $"Aturdido:{d.TurnosAturdido}" : "Activo"
                    });
                }

                foreach (Point n in nucleosEnergia)
                {
                    entidadesSnapshot.Add(new EntidadMapaDTO
                    {
                        TipoEntidad = "NucleoEnergia",
                        PosX = n.X,
                        PosY = n.Y,
                        Vida = 1,
                        Estado = "Activo"
                    });
                }

                entidadesSnapshot.Add(new EntidadMapaDTO
                {
                    TipoEntidad = "RoverAliado",
                    PosX = robotAliado.X,
                    PosY = robotAliado.Y,
                    Vida = 100,
                    Estado = dirRobot >= 0 ? "Derecha" : "Izquierda"
                });

                PartidaActivaSQL.Entidades = entidadesSnapshot;

                if (_gestorBD.GuardarProgresoPartida(PartidaActivaSQL))
                {
                    estadoSyncSQL = $"SQL: GUARDADO ({DateTime.Now:HH:mm:ss})";
                    if (!silencioso)
                    {
                        ultimoEvento = $"[BÓVEDA SQL] PROGRESO RESPALDADO EN RANURA '{PartidaActivaSQL.NombreRanura}' (TURNO {turnosJugados}).";
                    }
                    this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                estadoSyncSQL = "SQL: ERROR DE ENLACE";
                if (!silencioso)
                {
                    MessageBox.Show($"Error al sincronizar el traje RIG con SQL Server:\n{ex.Message}",
                        "Fallo de Bóveda SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InicializarEntidades()
        {
            paredes.Clear();
            paredes.Add(new Point(2, 1));
            paredes.Add(new Point(2, 2));
            paredes.Add(new Point(2, 3));
            paredes.Add(new Point(5, 2));
            paredes.Add(new Point(6, 2));
            paredes.Add(new Point(7, 2));
            paredes.Add(new Point(9, 4));
            paredes.Add(new Point(9, 5));
            paredes.Add(new Point(9, 6));
            paredes.Add(new Point(4, 6));
            paredes.Add(new Point(5, 6));
            paredes.Add(new Point(4, 7));
            paredes.Add(new Point(11, 2));
            paredes.Add(new Point(11, 3));
            paredes.Add(new Point(11, 8));
            paredes.Add(new Point(12, 8));
            paredes.Add(new Point(12, 9));

            cadeteActual.PosX = Math.Clamp(cadeteActual.PosX, 0, COLS - 1);
            cadeteActual.PosY = Math.Clamp(cadeteActual.PosY, 0, FILAS - 1);
            if (paredes.Contains(new Point(cadeteActual.PosX, cadeteActual.PosY)))
            {
                cadeteActual.PosX = 0;
                cadeteActual.PosY = 0;
            }

            enemigos.Clear();
            enemigos.Add(new DronEnemigo(6, 3, blindado: false));
            enemigos.Add(new DronEnemigo(10, 5, blindado: true));
            enemigos.Add(new DronEnemigo(8, 9, blindado: false));
            enemigos.Add(new DronEnemigo(13, 2, blindado: true));

            nucleosEnergia.Clear();
            nucleosEnergia.Add(new Point(4, 2));
            nucleosEnergia.Add(new Point(11, 7));
            nucleosEnergia.Add(new Point(3, 7));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int nuevoX = cadeteActual.PosX;
            int nuevoY = cadeteActual.PosY;
            bool intentoMover = false;

            switch (keyData)
            {
                case Keys.W:
                case Keys.Up:
                    dirX = 0; dirY = -1;
                    if (nuevoY > 0) { nuevoY--; intentoMover = true; }
                    break;
                case Keys.S:
                case Keys.Down:
                    dirX = 0; dirY = 1;
                    if (nuevoY < FILAS - 1) { nuevoY++; intentoMover = true; }
                    break;
                case Keys.A:
                case Keys.Left:
                    dirX = -1; dirY = 0;
                    if (nuevoX > 0) { nuevoX--; intentoMover = true; }
                    break;
                case Keys.D:
                case Keys.Right:
                    dirX = 1; dirY = 0;
                    if (nuevoX < COLS - 1) { nuevoX++; intentoMover = true; }
                    break;
                case Keys.Space:
                    EjecutarPulsoTactico();
                    this.Invalidate();
                    return true;
                    turnosJugados++;
                    EjecutarPulsoTactico();
                    this.Invalidate();
                    return true;
                case Keys.G:
                    SincronizarEstadoConSQL(silencioso: false);
                    return true;
                case Keys.Escape:
                    this.Close();
                    return true;
            }

            if (intentoMover)
            {
                Point destino = new Point(nuevoX, nuevoY);

                if (paredes.Contains(destino))
                {
                    ultimoEvento = $"[COBERTURA] MURO BLINDADO EN ({nuevoX},{nuevoY}) — PASO BLOQUEADO.";
                    this.Invalidate();
                    return true;
                }

                turnosJugados++;
                cadeteActual.PosX = nuevoX;
                cadeteActual.PosY = nuevoY;
                VerificarColisionesJugador();
                this.Invalidate();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void EjecutarPulsoTactico()
        {
            if (cargasEMP <= 0)
            {
                ultimoEvento = "[ALERTA RIG] SIN CARGAS EMP — RECOLECTA UN NÚCLEO DE ENERGÍA PARA RECARGAR.";
                return;
            }

            cargasEMP--;
            turnosJugados++;
            pulsoVisualFrames = 5;
            int destruidos = 0;
            int aturdidos = 0;

            for (int i = enemigos.Count - 1; i >= 0; i--)
            {
                int dx = Math.Abs(enemigos[i].Posicion.X - cadeteActual.PosX);
                int dy = Math.Abs(enemigos[i].Posicion.Y - cadeteActual.PosY);

                if (dx <= 1 && dy <= 1)
                {
                    enemigos[i].Vida--;
                    enemigos[i].ApuntandoLaser = false;
                    enemigos[i].TurnosAturdido = 3;

                    if (enemigos[i].Vida <= 0)
                    {
                        int bono = enemigos[i].Sobrecargado ? 100 : (enemigos[i].VidaMax > 1 ? 75 : 50);
                        xpActual += bono;
                        enemigos.RemoveAt(i);
                        destruidos++;
                    }
                    else
                    {
                        aturdidos++;
                    }
                }
            }

            if (destruidos > 0 || aturdidos > 0)
            {
                ultimoEvento = $"[PULSO EMP ({cargasEMP}/{MAX_CARGAS_EMP})] {destruidos} FUNDIDO(S) | {aturdidos} ATURDIDO(S).";
                if (destruidos > 0) GenerarNuevoEnemigo();
            }
            else
            {
                ultimoEvento = $"[PULSO EMP ({cargasEMP}/{MAX_CARGAS_EMP})] DESCARGA EMITIDA — SIN HOSTILES EN RANGO 3x3.";
            }
        }

        private void GenerarNuevoEnemigo()
        {
            for (int intentos = 0; intentos < 30; intentos++)
            {
                int x = rng.Next(4, COLS);
                int y = rng.Next(0, FILAS);
                Point p = new Point(x, y);

                if (!paredes.Contains(p) && p != naveExtraccion && (x != cadeteActual.PosX || y != cadeteActual.PosY))
                {
                    bool esBlindado = rng.Next(0, 100) < 40;
                    enemigos.Add(new DronEnemigo(x, y, esBlindado));
                    break;
                }
            }
        }

        private void VerificarColisionesJugador()
        {
            Point posJugador = new Point(cadeteActual.PosX, cadeteActual.PosY);

            for (int i = nucleosEnergia.Count - 1; i >= 0; i--)
            {
                if (nucleosEnergia[i] == posJugador)
                {
                    nucleosEnergia.RemoveAt(i);
                    cargasEMP = Math.Min(MAX_CARGAS_EMP, cargasEMP + 1);
                    xpActual += 25;
                    vidaActual = Math.Min(vidaMaxActual, vidaActual + 15);
                    ReubicarNuevoNucleo();
                    SincronizarEstadoConSQL(silencioso: true);
                    ultimoEvento = "[NÚCLEO RECOLECTADO] +25 XP | +15 HP | PROGRESO SINCRONIZADO EN SQL.";
                }
            }

            for (int i = 0; i < enemigos.Count; i++)
            {
                if (enemigos[i].Posicion == posJugador && enemigos[i].TurnosAturdido == 0)
                {
                    int dano = enemigos[i].Sobrecargado ? 25 : 15;
                    AplicarDanoJugador(dano, $"[IMPACTO FÍSICO] COLISIÓN CON DRON HOSTIL (-{dano} HP).");
                    if (vidaActual == 0) return;
                }
            }

            if (posJugador == naveExtraccion)
            {
                xpActual += 100;

                ultimoEvento = "[EXTRACCIÓN COMPLETADA] +100 XP DE MISIÓN CUMPLIDA.";
                SincronizarEstadoConSQL(silencioso: true);
                tmrSimulacion.Stop();
                MessageBox.Show($"Extracción exitosa en la Nave.\nXP Total del Operador: {xpActual} PTS\nProgreso respaldado en SQL Server.",
                    "Misión Cumplida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void ReubicarNuevoNucleo()
        {
            Point nuevoNucleo;
            int intentos = 0;
            do
            {
                nuevoNucleo = new Point(rng.Next(0, COLS), rng.Next(0, FILAS));
                intentos++;
            } while (intentos < 40 && (paredes.Contains(nuevoNucleo) || nuevoNucleo == naveExtraccion || (nuevoNucleo.X == cadeteActual.PosX && nuevoNucleo.Y == cadeteActual.PosY)));

            nucleosEnergia.Add(nuevoNucleo);
        }

        private void AplicarDanoJugador(int cantidad, string mensaje)
        {
            vidaActual = Math.Max(0, vidaActual - cantidad);
            danoFlashFrames = 4;
            ultimoEvento = mensaje;

            if (vidaActual == 0)
            {
                tmrSimulacion.Stop();
                MessageBox.Show("Integridad del operador crítica (0 HP). Regresando a la base.", "Simulación Finalizada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vidaActual = 50;
                this.Close();
            }
        }

        private bool TieneLineaDeVista(Point a, Point b)
        {
            if (a.X == b.X)
            {
                int minY = Math.Min(a.Y, b.Y);
                int maxY = Math.Max(a.Y, b.Y);
                for (int y = minY + 1; y < maxY; y++)
                {
                    if (paredes.Contains(new Point(a.X, y))) return false;
                }
                return true;
            }
            if (a.Y == b.Y)
            {
                int minX = Math.Min(a.X, b.X);
                int maxX = Math.Max(a.X, b.X);
                for (int x = minX + 1; x < maxX; x++)
                {
                    if (paredes.Contains(new Point(x, a.Y))) return false;
                }
                return true;
            }
            return false;
        }

        private void tmrSimulacion_Tick(object? sender, EventArgs e)
        {
            ticksSimulacion++;
            animFrame = (animFrame + 1) % 360;

            if (pulsoVisualFrames > 0) pulsoVisualFrames--;
            if (danoFlashFrames > 0) danoFlashFrames--;

            for (int i = rayosActivos.Count - 1; i >= 0; i--)
            {
                rayosActivos[i].FramesRestantes--;
                if (rayosActivos[i].FramesRestantes <= 0) rayosActivos.RemoveAt(i);
            }

            if (ticksSimulacion % 7 == 0)
            {
                ProcesarIAEnemigos();
                ProcesarRoverAliado();
            }

            this.Invalidate();
        }

        private void ProcesarIAEnemigos()
        {
            Point posJugador = new Point(cadeteActual.PosX, cadeteActual.PosY);

            for (int i = 0; i < enemigos.Count; i++)
            {
                DronEnemigo dron = enemigos[i];

                if (dron.TurnosAturdido > 0)
                {
                    dron.TurnosAturdido--;
                    dron.ApuntandoLaser = false;
                    continue;
                }

                if (dron.ApuntandoLaser)
                {
                    dron.ApuntandoLaser = false;
                    rayosActivos.Add(new RayoVisual
                    {
                        Inicio = dron.Posicion,
                        Fin = dron.ObjetivoLaser,
                        ColorRayo = dron.Sobrecargado ? Color.Magenta : Color.OrangeRed,
                        FramesRestantes = 4
                    });

                    if (posJugador == dron.ObjetivoLaser)
                    {
                        int danoLaser = dron.Sobrecargado ? 20 : 10;
                        AplicarDanoJugador(danoLaser, $"[IMPACTO LÁSER] ALCANZADO POR DRON HOSTIL (-{danoLaser} HP).");
                        if (vidaActual == 0) return;
                    }
                    continue;
                }

                int distJugador = Math.Abs(dron.Posicion.X - posJugador.X) + Math.Abs(dron.Posicion.Y - posJugador.Y);
                dron.EnAlerta = distJugador <= 5;

                if (distJugador <= 5 && TieneLineaDeVista(dron.Posicion, posJugador))
                {
                    dron.ApuntandoLaser = true;
                    dron.ObjetivoLaser = posJugador;
                    ultimoEvento = $"[AVISO DE MIRA] DRON EN ({dron.Posicion.X},{dron.Posicion.Y}) APUNTANDO LÁSER — ¡MUÉVETE!";
                    continue;
                }

                Point siguientePaso = dron.EnAlerta
                    ? CalcularPasoPersecucion(dron.Posicion, posJugador)
                    : CalcularPasoPatrulla(dron.Posicion);

                if (siguientePaso != dron.Posicion)
                {
                    dron.Posicion = siguientePaso;
                }

                for (int n = nucleosEnergia.Count - 1; n >= 0; n--)
                {
                    if (nucleosEnergia[n] == dron.Posicion)
                    {
                        nucleosEnergia.RemoveAt(n);
                        dron.Sobrecargado = true;
                        dron.Vida = Math.Min(3, dron.Vida + 1);
                        dron.VidaMax = Math.Max(dron.VidaMax, dron.Vida);
                        ultimoEvento = "[SABOTAJE] ¡UN DRON ABSORBIÓ UN NÚCLEO Y SE SOBRECARGÓ!";
                        ReubicarNuevoNucleo();
                    }
                }

                if (dron.Posicion == posJugador)
                {
                    int dano = dron.Sobrecargado ? 25 : 15;
                    AplicarDanoJugador(dano, $"[EMBOSCADA] DRON HOSTIL INTERCEPTÓ AL OPERADOR (-{dano} HP).");
                    if (vidaActual == 0) return;
                }
            }
        }

        private Point CalcularPasoPersecucion(Point origen, Point destino)
        {
            List<Point> candidatos = new List<Point>();

            int dx = Math.Sign(destino.X - origen.X);
            int dy = Math.Sign(destino.Y - origen.Y);

            if (dx != 0) candidatos.Add(new Point(origen.X + dx, origen.Y));
            if (dy != 0) candidatos.Add(new Point(origen.X, origen.Y + dy));

            candidatos.Add(new Point(origen.X + 1, origen.Y));
            candidatos.Add(new Point(origen.X - 1, origen.Y));
            candidatos.Add(new Point(origen.X, origen.Y + 1));
            candidatos.Add(new Point(origen.X, origen.Y - 1));

            foreach (Point p in candidatos)
            {
                if (EsCeldaTransitable(p)) return p;
            }
            return origen;
        }

        private Point CalcularPasoPatrulla(Point origen)
        {
            int dir = rng.Next(0, 4);
            Point p = origen;
            if (dir == 0) p.Y--;
            else if (dir == 1) p.Y++;
            else if (dir == 2) p.X--;
            else if (dir == 3) p.X++;

            return EsCeldaTransitable(p) ? p : origen;
        }

        private bool EsCeldaTransitable(Point p)
        {
            if (p.X < 0 || p.X >= COLS || p.Y < 0 || p.Y >= FILAS) return false;
            if (paredes.Contains(p) || p == naveExtraccion) return false;
            foreach (DronEnemigo otro in enemigos)
            {
                if (otro.Posicion == p) return false;
            }
            return true;
        }

        private void ProcesarRoverAliado()
        {
            int sigX = robotAliado.X + dirRobot;
            Point destinoRover = new Point(sigX, robotAliado.Y);

            if (sigX < 0 || sigX >= COLS - 2 || paredes.Contains(destinoRover))
            {
                dirRobot *= -1;
            }
            else
            {
                robotAliado = destinoRover;
            }

            for (int i = enemigos.Count - 1; i >= 0; i--)
            {
                int dist = Math.Abs(enemigos[i].Posicion.X - robotAliado.X) + Math.Abs(enemigos[i].Posicion.Y - robotAliado.Y);
                if (dist <= 2)
                {
                    rayosActivos.Add(new RayoVisual
                    {
                        Inicio = robotAliado,
                        Fin = enemigos[i].Posicion,
                        ColorRayo = Color.DeepSkyBlue,
                        FramesRestantes = 4
                    });

                    enemigos[i].Vida--;
                    if (enemigos[i].Vida <= 0)
                    {
                        xpActual += 30;
                        ultimoEvento = "[APOYO ROVER] TORRETA ALIADA ELIMINÓ UN DRON HOSTIL (+30 XP).";
                        enemigos.RemoveAt(i);
                        GenerarNuevoEnemigo();
                    }
                    else
                    {
                        ultimoEvento = "[APOYO ROVER] TORRETA ALIADA IMPACTÓ BLINDAJE ENEMIGO.";
                    }
                    break;
                }
            }
        }

        private void FormMapa2D_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using Font fontTitulo = new Font("Consolas", 10.5F, FontStyle.Bold);
            using Font fontHud = new Font("Consolas", 9F, FontStyle.Bold);
            using Font fontMini = new Font("Consolas", 8F);
            using Font fontEstado = new Font("Consolas", 7.5F, FontStyle.Bold);

            // 1. Barra superior HUD
            using (SolidBrush brHeader = new SolidBrush(Color.FromArgb(17, 24, 38)))
            {
                g.FillRectangle(brHeader, 0, 0, this.ClientSize.Width, 55);
            }
            using (Pen penHeaderLine = new Pen(Color.FromArgb(0, 180, 216), 1.5f))
            {
                g.DrawLine(penHeaderLine, 0, 55, this.ClientSize.Width, 55);
            }

            int nivelActual = 1 + (xpActual / 100);
            g.DrawString($"SECTOR: ({cadeteActual.PosX:D2},{cadeteActual.PosY:D2}) | TURNO: {turnosJugados}", fontTitulo, Brushes.Cyan, 25, 16);
            g.DrawString($"HP: {vidaActual}/{vidaMaxActual} | XP: {xpActual} (NV.{nivelActual}) | EMP: {cargasEMP}/{MAX_CARGAS_EMP}", fontTitulo, Brushes.SpringGreen, 390, 16);

            // 2. Tablero Táctico 15x12
            int mapW = COLS * TAM_CELDA;
            int mapH = FILAS * TAM_CELDA;

            using (SolidBrush brFondoMapa = new SolidBrush(Color.FromArgb(12, 18, 28)))
            {
                g.FillRectangle(brFondoMapa, OFFSET_X, OFFSET_Y, mapW, mapH);
            }

            using Pen penGrid = new Pen(Color.FromArgb(24, 42, 64), 1);
            using Pen penCross = new Pen(Color.FromArgb(40, 75, 110), 1);

            for (int c = 0; c <= COLS; c++)
            {
                int x = OFFSET_X + c * TAM_CELDA;
                g.DrawLine(penGrid, x, OFFSET_Y, x, OFFSET_Y + mapH);
            }
            for (int f = 0; f <= FILAS; f++)
            {
                int y = OFFSET_Y + f * TAM_CELDA;
                g.DrawLine(penGrid, OFFSET_X, y, OFFSET_X + mapW, y);
            }

            for (int c = 1; c < COLS; c += 2)
            {
                for (int f = 1; f < FILAS; f += 2)
                {
                    int ix = OFFSET_X + c * TAM_CELDA;
                    int iy = OFFSET_Y + f * TAM_CELDA;
                    g.DrawLine(penCross, ix - 2, iy, ix + 2, iy);
                    g.DrawLine(penCross, ix, iy - 2, ix, iy + 2);
                }
            }
            using (Pen penBordeMapa = new Pen(Color.DarkCyan, 1.5f))
            {
                g.DrawRectangle(penBordeMapa, OFFSET_X, OFFSET_Y, mapW, mapH);
            }

            // 3. Onda expansiva de pulso EMP
            if (pulsoVisualFrames > 0)
            {
                float progreso = (6 - pulsoVisualFrames) / 5f;
                int radio = (int)(TAM_CELDA * (0.8f + progreso * 1.1f));
                int cx = OFFSET_X + cadeteActual.PosX * TAM_CELDA + TAM_CELDA / 2;
                int cy = OFFSET_Y + cadeteActual.PosY * TAM_CELDA + TAM_CELDA / 2;

                using Pen penPulsoExt = new Pen(Color.FromArgb(190, 0, 255, 255), 2.5f);
                using Pen penPulsoInt = new Pen(Color.FromArgb(110, 120, 255, 255), 1.2f);
                g.DrawEllipse(penPulsoExt, cx - radio, cy - radio, radio * 2, radio * 2);
                g.DrawEllipse(penPulsoInt, cx - radio + 6, cy - radio + 6, (radio - 6) * 2, (radio - 6) * 2);
            }

            // 4. Líneas de mira láser hostiles
            using Pen penMira = new Pen(Color.FromArgb(200, 255, 60, 60), 1.5f) { DashStyle = DashStyle.Dash };
            foreach (DronEnemigo dron in enemigos)
            {
                if (dron.ApuntandoLaser)
                {
                    Point p1 = CentroCelda(dron.Posicion);
                    Point p2 = CentroCelda(dron.ObjetivoLaser);
                    g.DrawLine(penMira, p1, p2);
                    g.DrawEllipse(penMira, p2.X - 10, p2.Y - 10, 20, 20);
                }
            }

            // 5. Renderizado de Modelos 2D
            foreach (Point muro in paredes)
            {
                DibujarModeloMuro(g, OFFSET_X + muro.X * TAM_CELDA, OFFSET_Y + muro.Y * TAM_CELDA, TAM_CELDA);
            }

            DibujarModeloExtraccion(g, OFFSET_X + naveExtraccion.X * TAM_CELDA, OFFSET_Y + naveExtraccion.Y * TAM_CELDA, TAM_CELDA);

            foreach (Point n in nucleosEnergia)
            {
                DibujarModeloNucleo(g, OFFSET_X + n.X * TAM_CELDA, OFFSET_Y + n.Y * TAM_CELDA, TAM_CELDA);
            }

            DibujarModeloRobot(g, OFFSET_X + robotAliado.X * TAM_CELDA, OFFSET_Y + robotAliado.Y * TAM_CELDA, TAM_CELDA);

            foreach (DronEnemigo en in enemigos)
            {
                DibujarModeloEnemigo(g, OFFSET_X + en.Posicion.X * TAM_CELDA, OFFSET_Y + en.Posicion.Y * TAM_CELDA, TAM_CELDA, en, fontEstado);
            }

            DibujarModeloCadete(g, OFFSET_X + cadeteActual.PosX * TAM_CELDA, OFFSET_Y + cadeteActual.PosY * TAM_CELDA, TAM_CELDA);

            // 6. Rayos láser / plasma activos
            foreach (RayoVisual rayo in rayosActivos)
            {
                Point p1 = CentroCelda(rayo.Inicio);
                Point p2 = CentroCelda(rayo.Fin);
                using Pen penRayoExt = new Pen(Color.FromArgb(140, rayo.ColorRayo), 5f);
                using Pen penRayoInt = new Pen(Color.White, 1.8f);
                g.DrawLine(penRayoExt, p1, p2);
                g.DrawLine(penRayoInt, p1, p2);
            }

            // 7. Panel lateral de leyenda y bóveda SQL
            int panelX = OFFSET_X + mapW + 18;
            using (SolidBrush brSide = new SolidBrush(Color.FromArgb(17, 24, 38)))
            {
                g.FillRectangle(brSide, panelX, OFFSET_Y, 162, mapH);
            }
            g.DrawRectangle(Pens.DarkCyan, panelX, OFFSET_Y, 162, mapH);

            g.DrawString("MODELOS TÁCTICOS", fontHud, Brushes.Cyan, panelX + 10, OFFSET_Y + 12);

            int iconSize = 22;
            int iconX = panelX + 10;
            int textX = panelX + 38;

            DibujarModeloCadete(g, iconX, OFFSET_Y + 36, iconSize);
            g.DrawString("Cadete Exo", fontMini, Brushes.White, textX, OFFSET_Y + 41);

            DibujarModeloEnemigo(g, iconX, OFFSET_Y + 66, iconSize, new DronEnemigo(0, 0, false), fontEstado);
            g.DrawString("Dron Explor.", fontMini, Brushes.Tomato, textX, OFFSET_Y + 71);

            DibujarModeloEnemigo(g, iconX, OFFSET_Y + 96, iconSize, new DronEnemigo(0, 0, true), fontEstado);
            g.DrawString("Dron Blindado", fontMini, Brushes.Orange, textX, OFFSET_Y + 101);

            DibujarModeloRobot(g, iconX, OFFSET_Y + 126, iconSize);
            g.DrawString("Rover Torreta", fontMini, Brushes.DeepSkyBlue, textX, OFFSET_Y + 131);

            DibujarModeloNucleo(g, iconX, OFFSET_Y + 156, iconSize);
            g.DrawString("Núcleo (+XP)", fontMini, Brushes.Gold, textX, OFFSET_Y + 161);

            DibujarModeloMuro(g, iconX, OFFSET_Y + 186, iconSize);
            g.DrawString("Cobertura", fontMini, Brushes.LightSlateGray, textX, OFFSET_Y + 191);

            DibujarModeloExtraccion(g, iconX, OFFSET_Y + 216, iconSize);
            g.DrawString("Extracción", fontMini, Brushes.SpringGreen, textX, OFFSET_Y + 221);

            g.DrawString("CONTROLES", fontHud, Brushes.Cyan, panelX + 10, OFFSET_Y + 252);
            g.DrawString("W/A/S/D: Mover", fontMini, Brushes.LightGray, panelX + 10, OFFSET_Y + 274);
            g.DrawString("Espacio: EMP", fontMini, Brushes.Yellow, panelX + 10, OFFSET_Y + 292);
            g.DrawString("Tecla G: Guardar", fontMini, Brushes.SpringGreen, panelX + 10, OFFSET_Y + 310);
            g.DrawString("ESC: Base", fontMini, Brushes.LightGray, panelX + 10, OFFSET_Y + 328);

            g.DrawString("BÓVEDA MEMORIA", fontHud, Brushes.Gold, panelX + 10, OFFSET_Y + 352);
            g.DrawString(estadoSyncSQL, fontEstado, Brushes.SpringGreen, panelX + 10, OFFSET_Y + 416);

            // 8. Consola inferior de eventos
            int footerY = OFFSET_Y + mapH + 15;
            using (SolidBrush brFooter = new SolidBrush(Color.FromArgb(17, 24, 38)))
            {
                g.FillRectangle(brFooter, OFFSET_X, footerY, this.ClientSize.Width - 50, 32);
            }
            g.DrawRectangle(Pens.DarkCyan, OFFSET_X, footerY, this.ClientSize.Width - 50, 32);
            g.DrawString($"> {ultimoEvento}", fontHud, Brushes.Yellow, OFFSET_X + 10, footerY + 8);
        }

        private Point CentroCelda(Point gridPos)
        {
            return new Point(
                OFFSET_X + gridPos.X * TAM_CELDA + TAM_CELDA / 2,
                OFFSET_Y + gridPos.Y * TAM_CELDA + TAM_CELDA / 2
            );
        }

        // =========================================================================
        // MODELADOS VECTORIALES GDI+ DE ENTIDADES
        // =========================================================================

        private void DibujarModeloMuro(Graphics g, int x, int y, int size)
        {
            int pad = Math.Max(2, size / 14);
            Rectangle r = new Rectangle(x + pad, y + pad, size - pad * 2, size - pad * 2);

            using SolidBrush brBase = new SolidBrush(Color.FromArgb(46, 56, 72));
            using SolidBrush brCentro = new SolidBrush(Color.FromArgb(30, 38, 50));
            using Pen penBorde = new Pen(Color.FromArgb(120, 140, 165), 1.5f);
            using Pen penFranja = new Pen(Color.FromArgb(200, 160, 40), 1.5f);

            g.FillRectangle(brBase, r);
            g.DrawRectangle(penBorde, r);

            Rectangle inner = new Rectangle(r.X + 4, r.Y + 4, Math.Max(2, r.Width - 8), Math.Max(2, r.Height - 8));
            g.FillRectangle(brCentro, inner);
            g.DrawLine(penFranja, inner.Left + 2, inner.Bottom - 2, inner.Right - 2, inner.Top + 2);

            g.FillRectangle(Brushes.LightGray, r.X + 2, r.Y + 2, 2, 2);
            g.FillRectangle(Brushes.LightGray, r.Right - 4, r.Y + 2, 2, 2);
            g.FillRectangle(Brushes.LightGray, r.X + 2, r.Bottom - 4, 2, 2);
            g.FillRectangle(Brushes.LightGray, r.Right - 4, r.Bottom - 4, 2, 2);
        }

        private void DibujarModeloCadete(Graphics g, int x, int y, int size)
        {
            int cx = x + size / 2;
            int cy = y + size / 2;
            int r = size / 2 - 4;

            Color colorEscudo = danoFlashFrames > 0 ? Color.OrangeRed : Color.FromArgb(140, 0, 230, 255);
            using Pen penEscudo = new Pen(colorEscudo, 1.5f);
            g.DrawEllipse(penEscudo, cx - r - 1, cy - r - 1, (r + 1) * 2, (r + 1) * 2);

            using SolidBrush brArmadura = new SolidBrush(danoFlashFrames > 0 ? Color.Crimson : Color.FromArgb(0, 130, 165));
            using Pen penContorno = new Pen(Color.Cyan, 1.5f);
            g.FillEllipse(brArmadura, cx - r + 2, cy - r + 2, (r - 2) * 2, (r - 2) * 2);
            g.DrawEllipse(penContorno, cx - r + 2, cy - r + 2, (r - 2) * 2, (r - 2) * 2);

            int cr = Math.Max(3, r / 2);
            using SolidBrush brCasco = new SolidBrush(Color.FromArgb(225, 245, 255));
            g.FillEllipse(brCasco, cx - cr, cy - cr, cr * 2, cr * 2);

            int vx = cx + dirX * (cr - 1);
            int vy = cy + dirY * (cr - 1);
            using SolidBrush brVisor = new SolidBrush(Color.SpringGreen);
            g.FillEllipse(brVisor, vx - 3, vy - 3, 6, 6);
        }

        private void DibujarModeloEnemigo(Graphics g, int x, int y, int size, DronEnemigo dron, Font fontEstado)
        {
            int cx = x + size / 2;
            int cy = y + size / 2;
            int r = size / 2 - 5;

            if (size >= 30 && (dron.EnAlerta || dron.Sobrecargado))
            {
                Color colorRadar = dron.Sobrecargado ? Color.FromArgb(160, 220, 60, 255) : Color.FromArgb(140, 255, 60, 60);
                using Pen penRadar = new Pen(colorRadar, 1.3f);
                g.DrawEllipse(penRadar, x + 2, y + 2, size - 4, size - 4);
            }

            Color colorBrazos = dron.Sobrecargado ? Color.MediumOrchid : (dron.VidaMax > 1 ? Color.DarkOrange : Color.IndianRed);
            using Pen penBrazos = new Pen(colorBrazos, 2f);
            g.DrawLine(penBrazos, cx - r, cy - r, cx + r, cy + r);
            g.DrawLine(penBrazos, cx - r, cy + r, cx + r, cy - r);

            using Pen penRotor = new Pen(Color.FromArgb(180, 255, 130, 90), 1.2f);
            int rr = Math.Max(2, size / 9);
            g.DrawEllipse(penRotor, cx - r - rr, cy - r - rr, rr * 2, rr * 2);
            g.DrawEllipse(penRotor, cx + r - rr, cy - r - rr, rr * 2, rr * 2);
            g.DrawEllipse(penRotor, cx - r - rr, cy + r - rr, rr * 2, rr * 2);
            g.DrawEllipse(penRotor, cx + r - rr, cy + r - rr, rr * 2, rr * 2);

            int coreR = Math.Max(4, r - 2);
            Color colorNucleo = dron.Sobrecargado ? Color.DarkViolet : (dron.VidaMax > 1 ? Color.Chocolate : Color.Firebrick);
            using SolidBrush brCore = new SolidBrush(colorNucleo);
            using Pen penCore = new Pen(dron.TurnosAturdido > 0 ? Color.Cyan : Color.Gold, 1.4f);
            g.FillEllipse(brCore, cx - coreR, cy - coreR, coreR * 2, coreR * 2);
            g.DrawEllipse(penCore, cx - coreR, cy - coreR, coreR * 2, coreR * 2);

            if (size >= 30 && dron.Vida > 1)
            {
                g.DrawString(dron.Vida.ToString(), fontEstado, Brushes.White, cx - 4, cy - 6);
            }
        }

        private void DibujarModeloRobot(Graphics g, int x, int y, int size)
        {
            int pad = Math.Max(3, size / 7);
            Rectangle chasis = new Rectangle(x + pad, y + pad + 2, size - pad * 2, size - pad * 2 - 4);

            using SolidBrush brChasis = new SolidBrush(Color.FromArgb(30, 80, 130));
            using Pen penBorde = new Pen(Color.DeepSkyBlue, 1.5f);
            g.FillRectangle(brChasis, chasis);
            g.DrawRectangle(penBorde, chasis);

            int cx = x + size / 2;
            int cy = y + size / 2;
            int tr = Math.Max(3, size / 6);
            g.FillEllipse(Brushes.LightSkyBlue, cx - tr, cy - tr, tr * 2, tr * 2);
        }

        private void DibujarModeloNucleo(Graphics g, int x, int y, int size)
        {
            int cx = x + size / 2;
            int cy = y + size / 2;
            int r = Math.Max(4, size / 3);

            Point[] rombo =
            {
                new Point(cx, cy - r),
                new Point(cx + r, cy),
                new Point(cx, cy + r),
                new Point(cx - r, cy)
            };

            using SolidBrush brNucleo = new SolidBrush(Color.FromArgb(245, 158, 11));
            using Pen penBrillo = new Pen(Color.LightYellow, 1.5f);
            g.FillPolygon(brNucleo, rombo);
            g.DrawPolygon(penBrillo, rombo);
        }

        private void DibujarModeloExtraccion(Graphics g, int x, int y, int size)
        {
            int pad = Math.Max(3, size / 8);
            Rectangle zona = new Rectangle(x + pad, y + pad, size - pad * 2, size - pad * 2);

            using SolidBrush brZona = new SolidBrush(Color.FromArgb(20, 83, 45));
            using Pen penZona = new Pen(Color.SpringGreen, 1.8f);
            g.FillRectangle(brZona, zona);
            g.DrawRectangle(penZona, zona);

            int cx = x + size / 2;
            int cy = y + size / 2;
            int ir = Math.Max(3, size / 5);
            g.DrawEllipse(penZona, cx - ir, cy - ir, ir * 2, ir * 2);
        }
    }
}