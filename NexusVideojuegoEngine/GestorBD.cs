using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace NexusVideojuegoEngine
{
    public class GestorBD
    {
        private readonly string cadenaConexion = @"Server=(localdb)\MSSQLLocalDB;Database=NexusV65DB;Trusted_Connection=True;TrustServerCertificate=True;";

        public bool ProbarConexion()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR BD]: No se pudo conectar a SQL Server. Detalle: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Guarda el estado del Cadete y registra la Bitácora dentro de una Transacción T-SQL.
        /// Garantiza Atomicidad (ACID): si falla un registro, nada se guarda.
        /// </summary>
        public bool GuardarPartidaTransaccional(Cadete cadete, int energiaCore)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    // 1. Actualizar Datos del Cadete
                    string sqlCadete = @"UPDATE Cadetes 
                                         SET Rango = @Rango, Experiencia = @XP, VidaActual = @Vida, 
                                             VidaMax = @VidaMax, EnergiaCore = @Energia, FechaUltimoAcceso = GETDATE() 
                                         WHERE Nombre = @Nombre;";

                    using (SqlCommand cmd = new SqlCommand(sqlCadete, conexion, transaccion))
                    {
                        cmd.Parameters.AddWithValue("@Rango", cadete.Rango);
                        cmd.Parameters.AddWithValue("@XP", cadete.XP);
                        cmd.Parameters.AddWithValue("@Vida", cadete.Vida);
                        cmd.Parameters.AddWithValue("@VidaMax", cadete.VidaMax);
                        cmd.Parameters.AddWithValue("@Energia", energiaCore);
                        cmd.Parameters.AddWithValue("@Nombre", cadete.Nombre);
                        
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Si el cadete no existe aún en la BD, lo insertamos
                        if (filasAfectadas == 0)
                        {
                            string sqlInsert = @"INSERT INTO Cadetes (Nombre, Rango, Experiencia, VidaActual, VidaMax, EnergiaCore) 
                                                 VALUES (@Nombre, @Rango, @XP, @Vida, @VidaMax, @Energia);";
                            using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, conexion, transaccion))
                            {
                                cmdInsert.Parameters.AddWithValue("@Nombre", cadete.Nombre);
                                cmdInsert.Parameters.AddWithValue("@Rango", cadete.Rango);
                                cmdInsert.Parameters.AddWithValue("@XP", cadete.XP);
                                cmdInsert.Parameters.AddWithValue("@Vida", cadete.Vida);
                                cmdInsert.Parameters.AddWithValue("@VidaMax", cadete.VidaMax);
                                cmdInsert.Parameters.AddWithValue("@Energia", energiaCore);
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }

                    // 2. Registrar Evento de Persistencia en Bitácora
                    string sqlBitacora = @"INSERT INTO BitacoraEventos (CadeteId, Mensaje) 
                                           VALUES ((SELECT TOP 1 CadeteId FROM Cadetes WHERE Nombre = @Nombre), @Mensaje);";

                    using (SqlCommand cmdLog = new SqlCommand(sqlBitacora, conexion, transaccion))
                    {
                        cmdLog.Parameters.AddWithValue("@Nombre", cadete.Nombre);
                        cmdLog.Parameters.AddWithValue("@Mensaje", $"Guardado de partida exitoso — HP: {cadete.Vida}, Energía: {energiaCore}%");
                        cmdLog.ExecuteNonQuery();
                    }

                    // Confirmar Transacción (ACID - Commit)
                    transaccion.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Revertir Cambios en caso de error (ACID - Rollback)
                    transaccion.Rollback();
                    Console.WriteLine($"[ERROR PERSISTENCIA]: Transacción abortada. Detalle: {ex.Message}");
                    return false;
                }
            }
        }
    }
}