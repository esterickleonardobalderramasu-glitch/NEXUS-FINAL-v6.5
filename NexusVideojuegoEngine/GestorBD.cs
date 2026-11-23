using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NexusVideojuegoEngine.Models;

namespace NexusVideojuegoEngine.Data
{
    public class GestorBD
    {
        private readonly string _connectionString;

        public GestorBD(string? customConnectionString = null)
        {
            // IMPORTANTE: Si en SSMS tu servidor se llama .\SQLEXPRESS o distinto a localhost, cámbialo aquí:
            _connectionString = customConnectionString
                ?? @"Server=localhost;Database=DeadSignal_DB;Integrated Security=True;TrustServerCertificate=True;";
        }

        public (int IdJugador, bool EsNuevo) ObtenerORegistrarJugador(string nombreUsuario, string codigoCadete)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sqlBuscar = "SELECT IdJugador FROM Jugadores WHERE NombreUsuario = @nombre";
            using (SqlCommand cmdBuscar = new SqlCommand(sqlBuscar, conn))
            {
                cmdBuscar.Parameters.AddWithValue("@nombre", nombreUsuario.Trim());
                object? result = cmdBuscar.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string sqlUpd = "UPDATE Jugadores SET CodigoCadete = @codigo WHERE IdJugador = @id";
                    using SqlCommand cmdUpd = new SqlCommand(sqlUpd, conn);
                    cmdUpd.Parameters.AddWithValue("@codigo", codigoCadete.Trim());
                    cmdUpd.Parameters.AddWithValue("@id", Convert.ToInt32(result));
                    cmdUpd.ExecuteNonQuery();

                    return (Convert.ToInt32(result), false);
                }
            }

            string sqlInsertar = @"INSERT INTO Jugadores (NombreUsuario, CodigoCadete)
                                   OUTPUT INSERTED.IdJugador
                                   VALUES (@nombre, @codigo)";
            using (SqlCommand cmdInsertar = new SqlCommand(sqlInsertar, conn))
            {
                cmdInsertar.Parameters.AddWithValue("@nombre", nombreUsuario.Trim());
                cmdInsertar.Parameters.AddWithValue("@codigo", codigoCadete.Trim());
                int nuevoId = Convert.ToInt32(cmdInsertar.ExecuteScalar());
                return (nuevoId, true);
            }
        }

        public int CrearNuevaPartida(int idJugador, string nombreRanura, EstadoCadeteDTO estadoInicial, List<EntidadMapaDTO> entidadesIniciales)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string sqlPartida = @"INSERT INTO Partidas (IdJugador, NombreRanura, SectorActual, TurnosJugados, EstadoPartida)
                                      OUTPUT INSERTED.IdPartida
                                      VALUES (@idJug, @ranura, 'Cubierta 15x12', 0, 'Activa')";
                int idPartida;
                using (SqlCommand cmd = new SqlCommand(sqlPartida, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@idJug", idJugador);
                    cmd.Parameters.AddWithValue("@ranura", nombreRanura.Trim());
                    idPartida = Convert.ToInt32(cmd.ExecuteScalar());
                }

                InsertarEstadoCadete(conn, tx, idPartida, estadoInicial);
                ReemplazarEntidadesMapa(conn, tx, idPartida, entidadesIniciales);
                RegistrarBitacora(conn, tx, idPartida, "INICIO_MISION", $"Despliegue iniciado en ranura '{nombreRanura}'.");

                tx.Commit();
                return idPartida;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public bool GuardarProgresoPartida(PartidaCompletaDTO partida)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                string sqlPartida = @"UPDATE Partidas
                                      SET SectorActual = @sector,
                                          TurnosJugados = @turnos,
                                          UltimaActualizacion = GETDATE()
                                      WHERE IdPartida = @idPartida";
                using (SqlCommand cmd = new SqlCommand(sqlPartida, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@sector", partida.SectorActual);
                    cmd.Parameters.AddWithValue("@turnos", partida.TurnosJugados);
                    cmd.Parameters.AddWithValue("@idPartida", partida.IdPartida);
                    cmd.ExecuteNonQuery();
                }

                string sqlCadete = @"UPDATE EstadoCadete
                                     SET VidaActual = @vida,
                                         VidaMax = @vidaMax,
                                         ExperienciaXP = @xp,
                                         Nivel = @nivel,
                                         PosicionX = @x,
                                         PosicionY = @y,
                                         CargasEMP = @emp
                                     WHERE IdPartida = @idPartida";
                using (SqlCommand cmd = new SqlCommand(sqlCadete, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@vida", partida.Cadete.VidaActual);
                    cmd.Parameters.AddWithValue("@vidaMax", partida.Cadete.VidaMax);
                    cmd.Parameters.AddWithValue("@xp", partida.Cadete.ExperienciaXP);
                    cmd.Parameters.AddWithValue("@nivel", partida.Cadete.Nivel);
                    cmd.Parameters.AddWithValue("@x", partida.Cadete.PosicionX);
                    cmd.Parameters.AddWithValue("@y", partida.Cadete.PosicionY);
                    cmd.Parameters.AddWithValue("@emp", partida.Cadete.CargasEMP);
                    cmd.Parameters.AddWithValue("@idPartida", partida.IdPartida);
                    cmd.ExecuteNonQuery();
                }

                ReemplazarEntidadesMapa(conn, tx, partida.IdPartida, partida.Entidades);
                RegistrarBitacora(conn, tx, partida.IdPartida, "GUARDADO",
                    $"Sincronización RIG: Vida {partida.Cadete.VidaActual}, XP {partida.Cadete.ExperienciaXP}, Turno {partida.TurnosJugados}.");

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public DataSet ObtenerPartidasDataSet(int idJugador)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = @"SELECT p.IdPartida,
                                  p.NombreRanura AS [Ranura],
                                  p.SectorActual AS [Cubierta],
                                  p.TurnosJugados AS [Turnos],
                                  c.VidaActual AS [Vida RIG],
                                  c.ExperienciaXP AS [XP],
                                  c.Nivel,
                                  p.UltimaActualizacion AS [Última Sincronización]
                           FROM Partidas p
                           INNER JOIN EstadoCadete c ON p.IdPartida = c.IdPartida
                           WHERE p.IdJugador = @idJugador
                           ORDER BY p.UltimaActualizacion DESC";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
            adapter.SelectCommand.Parameters.AddWithValue("@idJugador", idJugador);

            DataSet ds = new DataSet();
            adapter.Fill(ds, "PartidasGuardadas");
            return ds;
        }

        public PartidaCompletaDTO? CargarPartidaCompleta(int idPartida)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            PartidaCompletaDTO? dto = null;

            string sqlCabecera = @"SELECT p.IdPartida, p.NombreRanura, p.SectorActual, p.TurnosJugados,
                                          c.VidaActual, c.VidaMax, c.ExperienciaXP, c.Nivel,
                                          c.PosicionX, c.PosicionY, c.CargasEMP
                                   FROM Partidas p
                                   INNER JOIN EstadoCadete c ON p.IdPartida = c.IdPartida
                                   WHERE p.IdPartida = @idPartida";

            using (SqlCommand cmd = new SqlCommand(sqlCabecera, conn))
            {
                cmd.Parameters.AddWithValue("@idPartida", idPartida);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dto = new PartidaCompletaDTO
                    {
                        IdPartida = reader.GetInt32(0),
                        NombreRanura = reader.GetString(1),
                        SectorActual = reader.GetString(2),
                        TurnosJugados = reader.GetInt32(3),
                        Cadete = new EstadoCadeteDTO
                        {
                            VidaActual = reader.GetInt32(4),
                            VidaMax = reader.GetInt32(5),
                            ExperienciaXP = reader.GetInt32(6),
                            Nivel = reader.GetInt32(7),
                            PosicionX = reader.GetInt32(8),
                            PosicionY = reader.GetInt32(9),
                            CargasEMP = reader.GetInt32(10)
                        }
                    };
                }
            }

            if (dto == null) return null;

            string sqlEntidades = @"SELECT TipoEntidad, PosX, PosY, Vida, Estado
                                    FROM EntidadesMapa
                                    WHERE IdPartida = @idPartida AND Estado <> 'Destruido' AND Estado <> 'Recogido'";
            using (SqlCommand cmdEnt = new SqlCommand(sqlEntidades, conn))
            {
                cmdEnt.Parameters.AddWithValue("@idPartida", idPartida);
                using SqlDataReader rEnt = cmdEnt.ExecuteReader();
                while (rEnt.Read())
                {
                    dto.Entidades.Add(new EntidadMapaDTO
                    {
                        TipoEntidad = rEnt.GetString(0),
                        PosX = rEnt.GetInt32(1),
                        PosY = rEnt.GetInt32(2),
                        Vida = rEnt.GetInt32(3),
                        Estado = rEnt.GetString(4)
                    });
                }
            }

            return dto;
        }

        public bool EliminarPartida(int idPartida)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string sql = "DELETE FROM Partidas WHERE IdPartida = @idPartida";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@idPartida", idPartida);
            return cmd.ExecuteNonQuery() > 0;
        }

        private void InsertarEstadoCadete(SqlConnection conn, SqlTransaction tx, int idPartida, EstadoCadeteDTO c)
        {
            string sql = @"INSERT INTO EstadoCadete (IdPartida, VidaActual, VidaMax, ExperienciaXP, Nivel, PosicionX, PosicionY, CargasEMP)
                           VALUES (@idPartida, @vida, @vidaMax, @xp, @nivel, @x, @y, @emp)";
            using SqlCommand cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@idPartida", idPartida);
            cmd.Parameters.AddWithValue("@vida", c.VidaActual);
            cmd.Parameters.AddWithValue("@vidaMax", c.VidaMax);
            cmd.Parameters.AddWithValue("@xp", c.ExperienciaXP);
            cmd.Parameters.AddWithValue("@nivel", c.Nivel);
            cmd.Parameters.AddWithValue("@x", c.PosicionX);
            cmd.Parameters.AddWithValue("@y", c.PosicionY);
            cmd.Parameters.AddWithValue("@emp", c.CargasEMP);
            cmd.ExecuteNonQuery();
        }

        private void ReemplazarEntidadesMapa(SqlConnection conn, SqlTransaction tx, int idPartida, List<EntidadMapaDTO> entidades)
        {
            using (SqlCommand cmdDel = new SqlCommand("DELETE FROM EntidadesMapa WHERE IdPartida = @id", conn, tx))
            {
                cmdDel.Parameters.AddWithValue("@id", idPartida);
                cmdDel.ExecuteNonQuery();
            }

            string sqlIns = @"INSERT INTO EntidadesMapa (IdPartida, TipoEntidad, PosX, PosY, Vida, Estado)
                              VALUES (@id, @tipo, @x, @y, @vida, @estado)";
            foreach (var ent in entidades)
            {
                using SqlCommand cmd = new SqlCommand(sqlIns, conn, tx);
                cmd.Parameters.AddWithValue("@id", idPartida);
                cmd.Parameters.AddWithValue("@tipo", ent.TipoEntidad);
                cmd.Parameters.AddWithValue("@x", ent.PosX);
                cmd.Parameters.AddWithValue("@y", ent.PosY);
                cmd.Parameters.AddWithValue("@vida", ent.Vida);
                cmd.Parameters.AddWithValue("@estado", ent.Estado);
                cmd.ExecuteNonQuery();
            }
        }

        private void RegistrarBitacora(SqlConnection conn, SqlTransaction tx, int idPartida, string accion, string detalle)
        {
            string sql = @"INSERT INTO BitacoraEventos (IdPartida, TipoAccion, Detalle)
                           VALUES (@id, @accion, @detalle)";
            using SqlCommand cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@id", idPartida);
            cmd.Parameters.AddWithValue("@accion", accion);
            cmd.Parameters.AddWithValue("@detalle", detalle);
            cmd.ExecuteNonQuery();
        }
    }
}