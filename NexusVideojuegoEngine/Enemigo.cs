using System;

namespace NexusVideojuegoEngine
{
    public class Enemigo : Personaje
    {
        public int DanoAtaque { get; private set; }

        public Enemigo(string nombre, int posX, int posY, int dano, int vida) : base(nombre, posX, posY, vida)
        {
            DanoAtaque = dano;
        }

        public bool MoverHacia(int targetX, int targetY, int[,] mapa)
        {
            if (!EstaVivo) return false;

            int dx = Math.Sign(targetX - PosX);
            int dy = Math.Sign(targetY - PosY);

            if (dx != 0 && EsCasillaTransitable(PosX + dx, PosY, mapa))
            {
                PosX += dx;
                return true;
            }
            else if (dy != 0 && EsCasillaTransitable(PosX, PosY + dy, mapa))
            {
                PosY += dy;
                return true;
            }

            return false;
        }

        private bool EsCasillaTransitable(int x, int y, int[,] mapa)
        {
            if (x < 0 || x >= mapa.GetLength(1) || y < 0 || y >= mapa.GetLength(0)) return false;
            int celda = mapa[y, x];
            return celda == 0 || celda == 2 || celda == 3 || celda == 4;
        }

        public override void ReaccionarAlAnalisis()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ANALIZADOR]: Entidad Hostil [{Nombre}]. Daño: {DanoAtaque} | HP: {Vida}/{VidaMax}.");
            Console.ResetColor();
        }
    }
}