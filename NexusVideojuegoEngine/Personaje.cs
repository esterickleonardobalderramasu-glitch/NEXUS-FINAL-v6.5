using System;

namespace NexusVideojuegoEngine
{
    public abstract class Personaje
    {
        public string Nombre { get; protected set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Vida { get; protected set; }
        public int VidaMax { get; protected set; }
        public bool EstaVivo => Vida > 0;

        public Personaje(string nombre, int posX, int posY, int vida)
        {
            Nombre = nombre;
            PosX = posX;
            PosY = posY;
            Vida = Math.Max(1, vida);
            VidaMax = Math.Max(1, vida);
        }

        public abstract void ReaccionarAlAnalisis();

        public virtual void RecibirDano(int cantidad)
        {
            Vida = Math.Max(0, Vida - cantidad);
        }

        public void Curar(int cantidad)
        {
            Vida = Math.Min(VidaMax, Vida + cantidad);
        }
    }
}