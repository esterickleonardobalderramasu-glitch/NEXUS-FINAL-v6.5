using System;

namespace NexusVideojuegoEngine
{
    public class Nave
    {
        public string Nombre { get; private set; }
        public int Combustible { get; private set; }
        public int SaludEscudo { get; private set; }
        public int CapacidadCarga { get; private set; }

        public Nave(string nombre, int combustible, int escudo, int capacidad)
        {
            Nombre = nombre;
            Combustible = Math.Clamp(combustible, 0, 100);
            SaludEscudo = Math.Clamp(escudo, 0, 100);
            CapacidadCarga = capacidad;
        }
    }
}