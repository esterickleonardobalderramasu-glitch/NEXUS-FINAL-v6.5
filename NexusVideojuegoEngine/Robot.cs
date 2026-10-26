using System;

namespace NexusVideojuegoEngine
{
    public class Robot : Personaje
    {
        public bool EscudoDefensivoActivo { get; private set; }
        public int Blindaje { get; private set; }

        public Robot(string nombre, int posX, int posY, int vida, int blindaje) : base(nombre, posX, posY, vida)
        {
            Blindaje = blindaje;
            EscudoDefensivoActivo = false;
        }

        public void ActivarProtocoloDefensivo()
        {
            EscudoDefensivoActivo = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[🤖 {Nombre}]: ¡Protocolo defensivo activado! Blindaje suplementario.");
            Console.ResetColor();
        }

        public override void RecibirDano(int cantidad)
        {
            if (EscudoDefensivoActivo) cantidad /= 2;
            base.RecibirDano(cantidad);
        }

        public override void ReaccionarAlAnalisis()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[ANALIZADOR]: Unidad Autómata Centinela [{Nombre}]. Blindaje: {Blindaje}%.");
            Console.ResetColor();
        }
    }
}