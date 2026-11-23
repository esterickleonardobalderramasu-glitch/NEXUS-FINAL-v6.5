using System.Collections.Generic;

namespace NexusVideojuegoEngine.Models
{
    public class EstadoCadeteDTO
    {
        public int VidaActual { get; set; }
        public int VidaMax { get; set; }
        public int ExperienciaXP { get; set; }
        public int Nivel { get; set; }
        public int PosicionX { get; set; }
        public int PosicionY { get; set; }
        public int CargasEMP { get; set; }
    }

    public class EntidadMapaDTO
    {
        public string TipoEntidad { get; set; } = string.Empty;
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Vida { get; set; }
        public string Estado { get; set; } = "Activo";
    }

    public class PartidaCompletaDTO
    {
        public int IdPartida { get; set; }
        public string NombreRanura { get; set; } = string.Empty;
        public string SectorActual { get; set; } = "Cubierta 15x12";
        public int TurnosJugados { get; set; }
        public EstadoCadeteDTO Cadete { get; set; } = new EstadoCadeteDTO();
        public List<EntidadMapaDTO> Entidades { get; set; } = new List<EntidadMapaDTO>();
    }
}