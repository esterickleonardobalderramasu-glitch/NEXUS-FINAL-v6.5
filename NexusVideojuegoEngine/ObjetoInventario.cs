using System;

namespace NexusVideojuegoEngine
{
    public enum TipoObjeto { Energia, Escudo, Ataque, Analizador }

    public class ObjetoInventario
    {
        public string Nombre { get; set; }
        public TipoObjeto Tipo { get; set; }
        public int Valor { get; set; }
        public string Icono { get; set; }

        public ObjetoInventario(string nombre, TipoObjeto tipo, int valor, string icono)
        {
            Nombre = nombre;
            Tipo = tipo;
            Valor = valor;
            Icono = icono;
        }
    }
}