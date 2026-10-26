using System;
using System.Collections.Generic;

namespace NexusVideojuegoEngine
{
    public class Cadete : Personaje
    {
        public int XP { get; private set; }
        public int Rango { get; private set; }
        public List<ObjetoInventario> Inventario { get; private set; }

        public Cadete(string nombre, int posX, int posY, int vida) : base(nombre, posX, posY, vida)
        {
            XP = 0;
            Rango = 1;
            Inventario = new List<ObjetoInventario>();
        }

        public void GanarXP(int cantidad)
        {
            XP += cantidad;
            if (XP >= Rango * 100)
            {
                Rango++;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n[★] ¡ASCENSO DE RANGO! Rango alcanzado: Nivel {Rango}");
                Console.ResetColor();
            }
        }

        public override void ReaccionarAlAnalisis()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[ANALIZADOR]: Registro Biométrico de {Nombre}. Cadete Explorer NEXUS - Año 2297.");
            Console.WriteLine($" Status: Vitalidad [{Vida}/{VidaMax}] HP | Rango: {Rango} | Experiencia: {XP} XP.");
            Console.ResetColor();
        }
    }
}